/*
using Common;
using ExitGames.Concurrency.Fibers;
using Photon.SocketServer;
using Space.Server;
using Space.Server.Events;
using Space.Server.Messages;
using System.Collections;
using GameMath;

namespace Space.Game
{
    class AsteroidItem : Item, IMmoItem
    {
        private Asteroid asteroid;
        private float[] position;
        private float[] rotation;

        public AsteroidItem(float[] position, float[] rotation, string id, ItemType type, MmoWorld world, IFiber fiber, Asteroid asteroid ) 
            : base(position.ToVector(false), new Hashtable(), id, type.toByte(), world, fiber, false)
        {
            this.asteroid = asteroid;
            this.position = position;
            this.rotation = rotation;
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

        public bool ReceiveEvent(Photon.SocketServer.EventData eventData, Photon.SocketServer.SendParameters sendParameters)
        {
            return false;
        }

        protected internal override ItemSnapshot GetItemSnapshot()
        {
            return new MmoItemSnapshot(this, this.position.ToVector(false), this.CurrentWorldRegion, this.PropertiesRevision, this.rotation, this.position);
        }

        protected override ItemPositionMessage GetPositionUpdateMessage(Vector position, Region region)
        {
            return new MmoItemPositionUpdate(this, this.position.ToVector(false), region, this.position);
        }

        public IActor GetOwner() { return this.asteroid; }
    }
}
*/
