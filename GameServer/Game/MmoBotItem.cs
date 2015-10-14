/*
using ExitGames.Concurrency.Fibers;
using Common;
using Space.Server;
using Space.Server.Events;
using Space.Server.Messages;
using Photon.SocketServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Space.Game.Maths;

namespace Space.Game
{
    public class MmoBotItem : Item, IMmoItem, IBotItem
    {
        private BotActor _owner;
        public float[] Coordinate { get; private set;  }
        public float[] Rotation { get; set;  }

        public BotActor Owner { get { return _owner; } }

        private byte _subType;

        public MmoBotItem(IWorld world, float[] coordinate, float[] rotation, Hashtable properties, BotActor owner, string itemId, byte itemType, byte botSubType, IFiber fiber ) 
            : base(coordinate.ToVector(), properties, itemId, itemType, world, fiber )
        {
            _owner = owner;
            this.Rotation = rotation;
            this.Coordinate = coordinate;
            _subType = botSubType;
        }

        public void Move(float[] coordinate)
        {
            this.Coordinate = coordinate;
            this.Position = coordinate.ToVector();
            this.UpdateInterestManagement();
        }

        public void Spawn(float[] coordinate)
        {
            this.Coordinate = coordinate;
            this.Position = coordinate.ToVector();
            this.UpdateInterestManagement();
        }

        public bool GrantWriteAccess(BotActor actor)
        {
            return _owner == actor;
        }

        public bool ReceiveEvent(EventData eventData, SendParameters sendParameters)
        {
            return true;
        }

        protected internal override ItemSnapshot GetItemSnapshot()
        {
            return new MmoItemSnapshot(this, this.Position, this.CurrentWorldRegion, this.PropertiesRevision, this.Rotation, this.Coordinate);
        }

        protected override ItemPositionMessage GetPositionUpdateMessage(Vector position, Region region)
        {
            return new MmoItemPositionUpdate(this, this.Position, region, this.Coordinate);
        }

        protected override void OnDestroy()
        {
            var eventInstance = new ItemDestroyed { ItemId = this.Id, ItemType = this.Type };
            var eventData = new EventData((byte)EventCode.ItemDestroyed, eventInstance);
            var message = new ItemEventMessage(this, eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
            this.EventChannel.Publish(message);
        }

        public bool GrantWriteAccess(MmoActor actor)
        {
            return true;
        }


        public IActor GetOwner()
        {
            return Owner;
        }



        public byte SubType
        {
            get { return this._subType; }
        }
    }
}
*/