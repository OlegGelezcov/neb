// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Actor.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Represents a player in a <see cref="IWorld">world</see>.
//   An actor can receive events using <see cref="InterestArea">interest areas</see> and publish events using <see cref="Item">items</see>.
//   <see cref="InterestArea">Interest areas</see> and <see cref="Item">items</see> can be added, removed and moved within the <see cref="IWorld">world</see>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server
{
    using Common;
    using Photon.SocketServer;
    using Space.Game;
    using GameMath;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nebula.Engine;
    using ExitGames.Logging;



    /// <summary>
    ///   Represents a player in a <see cref = "IWorld">world</see>. 
    ///   An actor can receive events using <see cref = "InterestArea">interest areas</see> and publish events using <see cref = "Item">items</see>.
    ///   <see cref = "InterestArea">Interest areas</see> and <see cref = "Item">items</see> can be added, removed and moved within the <see cref = "IWorld">world</see>.
    /// </summary>
    public abstract class Actor : NebulaBehaviour, IDisposable
    {
        #region Constants and Fields

        /// <summary>
        ///   The interest areas.
        /// </summary>
        private Dictionary<byte, InterestArea> interestAreas = new Dictionary<byte, InterestArea>();

        ///// <summary>
        /////   The owned items.
        ///// </summary>
        //private Dictionary<byte, Dictionary<string, Item>> ownedItems;

        /// <summary>
        ///   The peer.
        /// </summary>
        private PeerBase peer;

        ///// <summary>
        /////   The world.
        ///// </summary>
        //private IWorld world;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Actor" /> class.
        /// </summary>
        /// <param name = "peer">
        ///   The owner peer.
        /// </param>
        /// <param name = "world">
        ///   The world.
        /// </param>
        /// 
        private IWorld world;


        public override void Awake() {
        }

        public void SetPeer(PeerBase peer) {
            this.peer = peer;
        }

        public void SetWorld(IWorld world) {
            this.world = world;
        }



        /// <summary>
        ///   Finalizes an instance of the <see cref = "Actor" /> class.
        /// </summary>
        ~Actor()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the avatar <see cref = "Item" />.
        /// </summary>
        public Item Avatar { get; private set; }

        public void SetAvatar(Item avatar) {
            Avatar = avatar;
        }

        /// <summary>
        ///   Gets the owner peer.
        /// </summary>
        public PeerBase Peer
        {
            get
            {
                return this.peer;
            }
        }

        /// <summary>
        ///   Gets the <see cref = "IWorld">world</see> the actor is member of.
        /// </summary>
        public IWorld World {
            get {
                return this.world;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Adds an <see cref = "InterestArea" />.
        /// </summary>
        /// <param name = "interestArea">
        ///   The <see cref = "InterestArea" /> to add.
        /// </param>
        public void AddInterestArea(InterestArea interestArea)
        {
            this.interestAreas.Add(interestArea.Id, interestArea);
        }

        /// <summary>
        ///   Adds an <see cref = "Item" />.
        /// </summary>
        /// <param name = "item">
        ///   The <see cref = "Item" /> to add.
        /// </param>
        //public void AddItem(Item item)
        //{
        //    Dictionary<string, Item> typedItems;
        //    if (this.ownedItems.TryGetValue(item.Type, out typedItems) == false)
        //    {
        //        typedItems = new Dictionary<string, Item>();
        //        this.ownedItems.Add(item.Type, typedItems);
        //    }

        //    typedItems.Add(item.Id, item);
        //}

        /// <summary>
        ///   Gets the actor's <see cref = "InterestArea" />s.
        /// </summary>
        /// <returns>
        ///   A list of <see cref = "InterestArea" />s.
        /// </returns>
        public IEnumerable<InterestArea> GetInterestAreas()
        {
            return this.interestAreas.Values;
        }

        /// <summary>
        ///   Gets the type codes of the actor's <see cref = "Item" />s.
        /// </summary>
        /// <returns>
        ///   A list of item type codes.
        /// </returns>
        //public IEnumerable<byte> GetOwnedItemTypes()
        //{
        //    return this.ownedItems.Keys;
        //}

        /// <summary>
        ///   Gets the actor's <see cref = "Item" />s.
        /// </summary>
        /// <param name = "itemType">
        ///   The item type.
        /// </param>
        /// <returns>
        ///   A list of <see cref = "Item" />s.
        /// </returns>
        //public IEnumerable<Item> GetOwnedItems(byte itemType)
        //{
        //    Dictionary<string, Item> typedItems;
        //    if (this.ownedItems.TryGetValue(itemType, out typedItems))
        //    {
        //        return typedItems.Values;
        //    }

        //    return Enumerable.Empty<Item>();
        //}

        /// <summary>
        ///   Removes an <see cref = "InterestArea" />.
        /// </summary>
        /// <param name = "interestAreaId">
        ///   The interest area's id (<see cref = "InterestArea.Id" />).
        /// </param>
        /// <returns>
        ///   true if the <see cref = "InterestArea" /> was found.
        /// </returns>
        public bool RemoveInterestArea(byte interestAreaId)
        {
            return this.interestAreas.Remove(interestAreaId);
        }

        /// <summary>
        ///   Removes an <see cref = "Item" />.
        /// </summary>
        /// <param name = "item">
        ///   The removed item.
        /// </param>
        /// <returns>
        ///   true if item was found.
        /// </returns>
        public bool RemoveItem(Item item) {
            //Dictionary<string, Item> typedItems;
            //if (this.ownedItems.TryGetValue(item.Type, out typedItems)) {
            //    if (typedItems.Remove(item.Id)) {
            //        if (typedItems.Count == 0) {
            //            this.ownedItems.Remove(item.Type);
            //        }

            //        return true;
            //    }
            //}

            if(Avatar != null ) {
                Avatar = null;
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Tries to get an <see cref = "InterestArea" />.
        /// </summary>
        /// <param name = "interestAreaId">
        ///   The interest area id.
        /// </param>
        /// <param name = "interestArea">
        ///   The result <see cref = "InterestArea" />.
        /// </param>
        /// <returns>
        ///   true if the <see cref = "InterestArea" /> with the <paramref name = "interestAreaId" /> was found.
        /// </returns>
        public bool TryGetInterestArea(byte interestAreaId, out InterestArea interestArea)
        {
            return this.interestAreas.TryGetValue(interestAreaId, out interestArea);
        }

        /// <summary>
        ///   Tries to get an <see cref = "Item" />.
        /// </summary>
        /// <param name = "itemType">
        ///   The item type.
        /// </param>
        /// <param name = "itemid">
        ///   The item id.
        /// </param>
        /// <param name = "item">
        ///   The result <see cref = "Item" />.
        /// </param>
        /// <returns>
        ///   true if the <see cref = "Item" /> with the <paramref name = "itemType" /> and <paramref name = "itemid" /> was found.
        /// </returns>
        public bool TryGetItem(byte itemType, string itemid, out Item item) {
            if(Avatar != null ) {
                if(Avatar.Type == itemType && Avatar.Id == itemid) {
                    item = Avatar;
                    return true;
                }
            }
            //Dictionary<string, Item> typedItems;
            //if (this.ownedItems.TryGetValue(itemType, out typedItems)) {
            //    return typedItems.TryGetValue(itemid, out item);
            //}

            item = null;
            return false;
        }

        #endregion

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

        #region Methods

        private void ClearInteresetAreas() {
            foreach (InterestArea camera in this.interestAreas.Values) {
                lock (camera.SyncRoot) {
                    camera.Dispose();
                }
            }

            this.interestAreas.Clear();
        }

        /// <summary>
        ///   Disposes the <see cref = "InterestArea">interest areas</see> and destroys all owned <see cref = "Item">items</see>.
        /// </summary>
        /// <param name = "disposing">
        ///   True if called from <see cref = "Dispose()" />, false if called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            
            if (disposing)
            {
                ClearInteresetAreas();

                if(Avatar != null ) {
                    Avatar.Destroy();
                    Avatar.Dispose();
                    this.world.ItemCache.RemoveItem(Avatar.Type, Avatar.Id);
                }

                //foreach (Dictionary<string, Item> itemCache in this.ownedItems.Values)
                //{
                //    foreach (Item item in itemCache.Values)
                //    {
                //        item.Destroy();
                //        item.Dispose();
                //        this.world.ItemCache.RemoveItem(item.Type, item.Id);
                //    }
                //}

                //this.ownedItems.Clear();
                OnQuit();
            }
        }

        public virtual void OnQuit()
        {
        }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public void DeleteItemsFromWorld() 
        {
            foreach (InterestArea camera in this.interestAreas.Values) 
            {
                lock (camera.SyncRoot) 
                {
                    camera.Dispose();
                }
            }
            this.interestAreas.Clear();
            log.InfoFormat("clear actor interest areas green");
            if(Avatar != null ) {
                //Avatar.Destroy();
                //Avatar.Dispose();
                this.world.ItemCache.RemoveItem(Avatar.Type, Avatar.Id);
            }

            //foreach (Dictionary<string, Item> itemCache in this.ownedItems.Values) 
            //{
            //    foreach (Item item in itemCache.Values) 
            //    {
            //        item.Destroy();
            //        item.Dispose();
            //        this.world.ItemCache.RemoveItem(item.Type, item.Id);
            //    }
            //}
            //this.ownedItems.Clear();
        }

        #endregion

    }
}