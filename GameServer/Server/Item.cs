// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Represents an entity in a <see cref="IWorld">world</see>.
//   Items are event publisher and the counterpart to <see cref="InterestArea">InterestAreas</see>.
//   <para>
//   Items have
//   <list type="bullet">
//   <item>
//   a <see cref="Type" />,
//   </item>
//   <item>
//   a per type unique <see cref="Id" />,
//   </item>
//   <item>
//   a <see cref="Position" />,
//   </item>
//   <item>
//   <see cref="Properties" /> with a <see cref="PropertiesRevision">revision number</see>
//   </item>
//   <item>
//   and 3 different <see cref="MessageChannel{T}">MessageChannels</see>:
//   <list type="bullet">
//   <item>
//   <see cref="EventChannel" />: <see cref="EventData" /> for <see cref="ClientInterestArea">interest areas</see>.
//   </item>
//   <item>
//   <see cref="PositionUpdateChannel" />: Position updates for attached and subscribed <see cref="InterestArea">interest areas</see>.
//   Attached <see cref="InterestArea">interest areas</see> move to the same position and subscribed <see cref="InterestArea">interest areas</see>
//   unsubscribe when the item leaves the outer threshold (one position update is analyzed every few seconds).
//   </item>
//   <item>
//   <see cref="DisposeChannel" />: Subscribed <see cref="InterestArea">interest areas</see> are informed when the item is disposed in order to unsubscribe.
//   </item>
//   </list>
//   </item>
//   </list>
//   </para>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server
{
    using ExitGames.Concurrency.Fibers;
    using Photon.SocketServer.Concurrency;
    using Space.Server.Messages;
    using System;
    using System.Collections;
    using GameMath;
    using Common;
    using Nebula.Engine;
    using System.Collections.Generic;
    using Space.Game;
    using ExitGames.Logging;
    using Nebula.Game.Components;





    /// <summary>
    ///   Represents an entity in a <see cref = "IWorld">world</see>. 
    ///   Items are event publisher and the counterpart to <see cref = "InterestArea">InterestAreas</see>.
    ///   <para>
    ///     Items have
    ///     <list type = "bullet">
    ///       <item>
    ///         a <see cref = "Type" />,
    ///       </item>
    ///       <item>
    ///         a per type unique <see cref = "Id" />,
    ///       </item>
    ///       <item>
    ///         a <see cref = "Position" />,
    ///       </item>
    ///       <item>
    ///         <see cref = "Properties" /> with a <see cref = "PropertiesRevision">revision number</see>
    ///       </item>
    ///       <item>
    ///         and 3 different <see cref = "MessageChannel{T}">MessageChannels</see>: 
    ///         <list type = "bullet">
    ///           <item>
    ///             <see cref = "EventChannel" />: <see cref = "EventData" /> for <see cref = "ClientInterestArea">interest areas</see>.
    ///           </item>
    ///           <item>
    ///             <see cref = "PositionUpdateChannel" />: Position updates for attached and subscribed <see cref = "InterestArea">interest areas</see>. 
    ///             Attached <see cref = "InterestArea">interest areas</see> move to the same position and subscribed <see cref = "InterestArea">interest areas</see> 
    ///             unsubscribe when the item leaves the outer threshold (one position update is analyzed every few seconds).
    ///           </item>
    ///           <item>
    ///             <see cref = "DisposeChannel" />: Subscribed <see cref = "InterestArea">interest areas</see> are informed when the item is disposed in order to unsubscribe. 
    ///           </item>
    ///         </list>
    ///       </item>
    ///     </list>
    ///   </para>
    /// </summary>
    /// <remarks>
    ///   Item accessing operations are required to be invoked on the item's <see cref = "Fiber" />.
    /// </remarks>
    public class Item : NebulaObject, IDisposable
    {
#if MissingSubscribeDebug
        private static readonly ExitGames.Logging.ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
#endif

        /// <summary>
        ///   The dispose channel.
        /// </summary>
        private readonly MessageChannel<ItemDisposedMessage> disposeChannel;

        /// <summary>
        ///   The item eventChannel.
        /// </summary>
        private readonly MessageChannel<ItemEventMessage> eventChannel;

        /// <summary>
        ///   The fiber.
        /// </summary>
        protected readonly IFiber fiber;


        /// <summary>
        ///   The position region.
        /// </summary>
        private readonly MessageChannel<ItemPositionMessage> positionUpdateChannel;

        /// <summary>
        ///   The properties.
        /// </summary>
        //private readonly Hashtable properties;

        /// <summary>
        ///   The world.
        /// </summary>
        private readonly IWorld world;

        /// <summary>
        ///   The disposed.
        /// </summary>
        private bool disposed;

        private readonly object lockObject = new object();

        private bool fiberStarted = false;

        public bool destroyedByLogic { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Item" /> class.
        /// </summary>
        /// <param name = "position">
        ///   The position.
        /// </param>
        /// <param name = "properties">
        ///   The properties.
        /// </param>
        /// <param name = "id">
        ///   The id.
        /// </param>
        /// <param name = "type">
        ///   The type.
        /// </param>
        /// <param name = "world">
        ///   The world.
        /// </param>
        /// <param name = "fiber">
        ///   The fiber. Typically identical to the owner's <see cref = "PeerBase.RequestFiber">request fiber</see>.
        /// </param>
        public Item(Vector position, Hashtable props, string id, byte type, IWorld world, IFiber fiber, bool innerFiber, Dictionary<byte, object> inTags, float size, int subZone, params Type[] components)
            : this(position, props, id, type, world, inTags, size, subZone, components)
        {
            if (innerFiber)
            {
                this.fiber = new PoolFiber();
                this.fiberStarted = false;
            }
            else
            {
                this.fiber = fiber;
                this.fiberStarted = true;
            }

            //ServerRuntimeStats.Default.CreateItem(type);
        }

        public Item(Vector position, Hashtable props, string id, byte type, IWorld world, IFiber fiber, bool innerFiber, Dictionary<byte, object> inTags, float size, 
            int subZone, BehaviourCollection collection)
            : this(position, props, id, type, world, inTags, size, subZone, collection) {
            if (innerFiber) {
                this.fiber = new PoolFiber();
                this.fiberStarted = false;
            } else {
                this.fiber = fiber;
                this.fiberStarted = true;
            }

            //ServerRuntimeStats.Default.CreateItem(type);
        }

        private Item(Vector position, Hashtable props, string id, byte type, IWorld world, Dictionary<byte, object> inTags, float size, int subZone, params Type[] components)
            : base(world, id, type, inTags, size, subZone, components)
        {

            transform.SetPosition(position);
            properties.SetProperties(props, null);

            this.eventChannel = new MessageChannel<ItemEventMessage>(ItemEventMessage.CounterEventSend);
            this.disposeChannel = new MessageChannel<ItemDisposedMessage>(MessageCounters.CounterSend);
            this.positionUpdateChannel = new MessageChannel<ItemPositionMessage>(MessageCounters.CounterSend);
            destroyedByLogic = false;

            this.world = world;
        }

        private Item(Vector position, Hashtable props, string id, byte type, IWorld world, Dictionary<byte, object> inTags, float size, int subZone, BehaviourCollection collection)
            : base(world, id, type, inTags, size, subZone, collection) {
            transform.SetPosition(position);
            properties.SetProperties(props, null);

            this.eventChannel = new MessageChannel<ItemEventMessage>(ItemEventMessage.CounterEventSend);
            this.disposeChannel = new MessageChannel<ItemDisposedMessage>(MessageCounters.CounterSend);
            this.positionUpdateChannel = new MessageChannel<ItemPositionMessage>(MessageCounters.CounterSend);
            destroyedByLogic = false;

            this.world = world;
        }

        public void StartFiber()
        {
            if (!this.fiberStarted)
                if (this.fiber != null)
                {
                    this.fiber.Start();
                    this.fiberStarted = true;
                }
        }

        /// <summary>
        ///   Finalizes an instance of the <see cref = "Item" /> class. 
        ///   Suppressed by Dispose.
        /// </summary>
        ~Item()
        {
            this.Dispose(false);
        }

        /// <summary>
        ///   Gets the item fiber.
        /// </summary>
        public IFiber Fiber
        {
            get
            {
                return this.fiber;
            }
        }

        /// <summary>
        ///   Gets the <see cref = "Region" /> where at the item's current <see cref = "Position" />.
        /// </summary>
        public Region CurrentWorldRegion { get; private set; }

        /// <summary>
        ///   Gets the channel that is used to publish <see cref = "ItemDisposedMessage">dispose messages</see>.
        ///   Subscribed <see cref = "InterestArea">interest areas</see> unsubscribe when receiving the message.
        ///   <see cref = "Dispose(bool)" /> publishes the <see cref = "ItemDisposedMessage" /> message.
        /// </summary>
        public MessageChannel<ItemDisposedMessage> DisposeChannel
        {
            get
            {
                return this.disposeChannel;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this item has been disposed.
        ///   Actions that where enqueued on the <see cref = "Fiber" /> could arrive after the item has been disposed.
        ///   Check this property to ensure that your operation is legal.
        /// </summary>
        public bool Disposed
        {
            get
            {
                return this.disposed;
            }
        }

        /// <summary>
        ///   Gets the channel that is used to publish <see cref = "ItemEventMessage">ItemEventMessages</see>.
        ///   <see cref = "ClientInterestArea">ClientInterestAreas</see> subscribe this channel to forward all received <see cref = "EventData">events</see> to the client <see cref = "PeerBase" />.
        ///   <see cref = "ItemEventMessage">ItemEventMessages</see> are published by the developer's application.
        /// </summary>
        public MessageChannel<ItemEventMessage> EventChannel
        {
            get
            {
                return this.eventChannel;
            }
        }



        /// <summary>
        ///   Gets the channel that is used to publish <see cref = "ItemPositionMessage">ItemPositionMessages</see>.
        ///   Subscribed <see cref = "InterestArea">interest areas</see> use this channel to determine when to unsubscribe.
        ///   Attached <see cref = "InterestArea">interest areas</see> use this channel to update their current position accordingly.
        ///   <see cref = "ItemPositionMessage" /> are published with <see cref = "UpdateInterestManagement" />.
        /// </summary>
        public MessageChannel<ItemPositionMessage> PositionUpdateChannel
        {
            get
            {
                return this.positionUpdateChannel;
            }
        }


        /// <summary>
        ///   Gets or sets CurrentWorldRegionSubscription.
        /// </summary>
        private IDisposable CurrentWorldRegionSubscription { get; set; }

        /// <summary>
        ///   Does nothing but calling <see cref = "OnDestroy" />.
        /// </summary>
        public void Destroy()
        {
            OnGameLogicDeath();

            this.OnDestroy();
        }

        /// <summary>
        ///   Publishes a <see cref = "ItemPositionMessage" /> in the <see cref = "PositionUpdateChannel" /> 
        ///   and in the current <see cref = "Region" /> if it changes
        ///   and then updates the <see cref = "CurrentWorldRegion" />.
        /// </summary>
        public void UpdateInterestManagement()
        {
            Region newRegion = world.GetRegion(transform.position.ToVector());

            // inform attached and auto subscribed (delayed) interest areas
            ItemPositionMessage message = this.GetPositionUpdateMessage(transform.position.ToVector(), newRegion);
            this.positionUpdateChannel.Publish(message);
            //ConsoleLogging.Get.Print("position update channel count {0} on item {1}", positionUpdateChannel.NumSubscribers, (ItemType)Type);

            if (this.SetCurrentWorldRegion(newRegion))
            {
                // inform unsubscribed interest areas in new region
                ItemSnapshot snapshot = this.GetItemSnapshot();
                newRegion.Publish(snapshot);
            }
        }

        /// <summary>
        ///   Sets the <see cref = "Position" /> and calls <see cref = "UpdateInterestManagement" />.
        /// </summary>
        /// <param name = "position">
        ///   The position.
        /// </param>
        //[Obsolete("Use Position_set and UpdateInterestManagement() instead")]
        public void Move(Vector position)
        {
            transform.SetPosition(position);
            this.UpdateInterestManagement();
        }

        public void Move(Vector position, Vector rotation ) {
            transform.SetRotation(rotation);
            Move(position);
        }

        
        /// <summary>
        ///   Sets the <see cref = "Position" /> and calls <see cref = "UpdateInterestManagement" />.
        /// </summary>
        /// <param name = "position">
        ///   The new position.
        /// </param>
        [Obsolete("Use Position_set and UpdateInterestManagement() instead")]
        public void Spawn(Vector position)
        {
            transform.SetPosition(position);
            this.UpdateInterestManagement();
        }

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        ///   Calls <see cref = "Dispose(bool)" /> and suppresses finalization.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this);
        }

        #endregion

        #endregion

        /// <summary>
        ///   Requests an <see cref = "ItemSnapshot" />.
        /// </summary>
        /// <param name = "snapShotRequest">
        ///   The snap shot request.
        /// </param>
        internal void EnqueueItemSnapshotRequest(ItemSnapshotRequest snapShotRequest)
        {
            this.fiber.Enqueue(
                () =>
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    snapShotRequest.OnItemReceive(this);
                });
        }

        /// <summary>
        ///   Creates an <see cref = "ItemSnapshot" /> with a snapshot of the current attributes.
        ///   Override this method to return a subclass of <see cref = "ItemSnapshot" /> that includes more data.
        ///   The return value is published through the <see cref = "CurrentWorldRegion" /> or sent directly to an <see cref = "InterestArea" />.
        /// </summary>
        /// <returns>
        ///   A new <see cref = "ItemSnapshot" />.
        /// </returns>
        protected internal virtual ItemSnapshot GetItemSnapshot()
        {
            return new ItemSnapshot(this, transform.position.ToVector(), this.CurrentWorldRegion, properties.propertiesRevision);
        }

        /// <summary>
        ///   Publishes a <see cref = "ItemDisposedMessage" /> through the <see cref = "DisposeChannel" /> and disposes all channels.
        ///   <see cref = "Disposed" /> is set to true.
        /// </summary>
        /// <param name = "disposing">
        ///   True if called from <see cref = "Dispose()" />, false if called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SetCurrentWorldRegion(null);
                this.disposeChannel.Publish(new ItemDisposedMessage(this));
                this.eventChannel.Dispose();
                this.disposeChannel.Dispose();
                this.positionUpdateChannel.Dispose();

                
                this.disposed = true;
                //ServerRuntimeStats.Default.DisposeItem(this.Type);
            }
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        public void ClearChannels() {
            log.InfoFormat("item Clear channels called green");
            eventChannel.ClearSubscribers();
            disposeChannel.ClearSubscribers();
            positionUpdateChannel.ClearSubscribers();
        }

        /// <summary>
        ///   Creates an <see cref = "ItemPositionMessage" /> with the current position and region.
        ///   The return value is published through the <see cref = "PositionUpdateChannel" />.
        /// </summary>
        /// <param name = "position">
        ///   The position.
        /// </param>
        /// <param name = "region">
        ///   The region.
        /// </param>
        /// <returns>
        ///   An instance of <see cref = "ItemPositionMessage" />.
        /// </returns>
        protected virtual ItemPositionMessage GetPositionUpdateMessage(Vector position, Region region)
        {
            return new ItemPositionMessage(this, position, region);
        }

        /// <summary>
        ///   Called from <see cref = "Destroy" />.
        ///   Does nothing.
        /// </summary>
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        ///   The set current world region.
        /// </summary>
        /// <param name = "newRegion">
        ///   The new region.
        /// </param>
        /// <returns>
        ///   True if the current region changed.
        /// </returns>
        protected bool SetCurrentWorldRegion(Region newRegion)
        {
            // out of bounds
            if (newRegion == null)
            {
                // was not out of bounce before
                if (this.CurrentWorldRegion != null)
                {
#if MissingSubscribeDebug
                    if (log.IsDebugEnabled)
                    {
                        log.DebugFormat("{0} unsubscribed from region {1}", this.id, this.CurrentWorldRegion.Coordinate);
                    }

#endif
                    this.CurrentWorldRegion = null;
                    this.CurrentWorldRegionSubscription.Dispose();
                    this.CurrentWorldRegionSubscription = null;
                }

#if MissingSubscribeDebug
                else if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0} out of bounds", this.id);
                }
#endif

                return false;
            }

            // was out of bounce before
            if (this.CurrentWorldRegion == null)
            {
                this.CurrentWorldRegionSubscription = newRegion.Subscribe(this.fiber, this.Region_OnReceive);
#if MissingSubscribeDebug
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0} subscribed to region {1} - before null", this.id, newRegion.Coordinate);
                }
#endif
                this.CurrentWorldRegion = newRegion;
                return true;
            }

            // current region changed
            if (newRegion != this.CurrentWorldRegion)
            {
                IDisposable newSubscription = newRegion.Subscribe(this.fiber, this.Region_OnReceive);
#if MissingSubscribeDebug
                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("{0} subscribed to region {1} - before {2}", this.id, newRegion.Coordinate, this.CurrentWorldRegion.Coordinate);
                }
#endif
                this.CurrentWorldRegionSubscription.Dispose();
                this.CurrentWorldRegionSubscription = newSubscription;
                this.CurrentWorldRegion = newRegion;
                return true;
            }

            return false;
        }

        /// <summary>
        ///   The on region message receive.
        /// </summary>
        /// <param name = "message">
        ///   The message.
        /// </param>
        private void Region_OnReceive(RegionMessage message)
        {
            if (this.disposed)
            {
                return;
            }

            message.OnItemReceive(this);
        }

        protected override bool Valid() {
            return (!Disposed) && (!destroyedByLogic);
        }

        public virtual void OnGameLogicDeath() {

            if (!destroyedByLogic) {

                destroyedByLogic = true;
                
                SendMessage(ComponentMessages.Death);
            }
        }
    }
}