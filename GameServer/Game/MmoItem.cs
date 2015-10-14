namespace Space.Game {
    using Common;
    using GameMath;
    using Nebula.Engine;
    using Photon.SocketServer;
    using Space.Server;
    using Space.Server.Events;
    using Space.Server.Messages;
    using System.Collections;
    using System.Collections.Generic;

    public class MmoItem : Item, IMmoItem
    {
        private MmoActor owner;

        public MmoItem(PeerBase peer, InterestArea interest, IWorld world, GameApplication app,  float[] coordinate, float[] rotation, 
            Hashtable properties, string gameRefId, Dictionary<byte, object> inTags, float size, int subZone, params System.Type[] components)
            : base(coordinate.ToVector(false), properties, gameRefId, (byte)ItemType.Avatar, world, peer.RequestFiber, false, inTags, size, subZone, components)
        {

            owner = GetComponent<MmoActor>();
            owner.SetApplication(app);
            owner.SetPeer(peer);
            owner.SetAvatar(this);
            owner.AddInterestArea(interest);
            owner.SetWorld(world);
            transform.SetPosition(coordinate);
            transform.SetRotation(rotation);
        }

        public MmoItem(PeerBase peer, InterestArea interest, IWorld world, GameApplication app, float[] coordinate, float[] rotation,
    Hashtable properties, string gameRefId, Dictionary<byte, object> inTags, float size, int subZone, BehaviourCollection collection)
    : base(coordinate.ToVector(false), properties, gameRefId, (byte)ItemType.Avatar, world, peer.RequestFiber, false, inTags, size, subZone, collection) {

            owner = GetComponent<MmoActor>();
            owner.SetApplication(app);
            owner.SetPeer(peer);
            owner.SetAvatar(this);
            owner.AddInterestArea(interest);
            owner.SetWorld(world);
            transform.SetPosition(coordinate);
            transform.SetRotation(rotation);
        }


        //public float[] Coordinate { get; private set; }
        public MmoActor Owner
        {
            get { return owner; }
        }

        //public float[] Rotation { get; set; }

        public void Move(float[] coordinate )
        {
            //this.Coordinate = coordinate;
            transform.SetPosition(coordinate.ToVector(false));
            this.UpdateInterestManagement();
        }

        public void Spawn(float[] coordinate)
        {
            //this.Coordinate = coordinate;
            transform.SetPosition(coordinate.ToVector(false));
            this.UpdateInterestManagement();
        }

        public bool GrantWriteAccess(MmoActor actor )
        {
            return this.owner == actor;
        }

        public bool ReceiveEvent(EventData eventData, SendParameters sendParameters )
        {
            this.owner.Peer.SendEvent(eventData, sendParameters);
            return true;
        }

        
        protected internal override ItemSnapshot GetItemSnapshot()
        {
            return new MmoItemSnapshot(this, transform.position.ToVector(), this.CurrentWorldRegion, this.properties.propertiesRevision, 
                transform.rotation.ToArray(), 
                transform.position.ToArray());
        }


        protected override ItemPositionMessage GetPositionUpdateMessage(Vector position, Region region)
        {
            return new MmoItemPositionUpdate(this, transform.position.ToVector(), region, transform.position.ToArray());
        }

        protected override void OnDestroy()
        {
            var eventInstance = new ItemDestroyed { ItemId = this.Id, ItemType = this.Type };
            var eventData = new EventData((byte)EventCode.ItemDestroyed, eventInstance);
            var message = new ItemEventMessage(this, eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
            this.EventChannel.Publish(message);
        }

    }
}
