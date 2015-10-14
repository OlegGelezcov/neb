
namespace Space.Game
{
    using Common;
    using Space.Server.Events;
    using Space.Server.Messages;
    using Photon.SocketServer;
    using Space.Server;
    using Nebula.Game.Components;
    using ExitGames.Logging;

    public class MmoClientInterestArea : ClientInterestArea
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public MmoClientInterestArea(PeerBase peer, byte id, IWorld world )
            : base(peer, id, world, peer.RequestFiber )
        { }

        protected override void OnItemSubscribed(ItemSnapshot snapshot)
        {
            //ConsoleLogging.Get.Print("ON PLAYER ITEM SUBSSCRIBED: {0}, {1}", snapshot.Source.Id.Substring(0, 3), (ItemType)snapshot.Source.Type);
            base.OnItemSubscribed(snapshot);
            var mmoSnapshot = (MmoItemSnapshot)snapshot;
            var item = snapshot.Source;

            if((bool)item == false ) {
                return;
            } 

            byte subType = (byte)0;

            if(item.GetComponent<BotObject>()) {
                subType = item.GetComponent<BotObject>().botSubType;
            }

            string displayName = item.name;

            //log.InfoFormat("item subscribed position: {0},{1},{2}", item.transform.position.X, item.transform.position.Y, item.transform.position.Z);

            var subscribeEvent = new ItemSubscribed
            {
                ItemId = item.Id,
                ItemType = item.Type,
                Position = item.transform.position.ToArray(),  //mmoSnapshot.Coordinate,
                PropertiesRevision = snapshot.PropertiesRevision,
                InterestAreaId = this.Id,
                Rotation = mmoSnapshot.Rotation,
                SubType = subType,
                Properties = item.properties.raw,
                DisplayName = displayName,
                Components = item.componentIds,
                size = item.size,
                subZone = item.subZone
            };
            var eventData = new EventData((byte)EventCode.ItemSubscribed, subscribeEvent);
            this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
            //log.InfoFormat("SEND ITEM SUBSCRIBE {0}:{1}  ITEM VALID: {2}", (ItemType)item.Type, item.Id, (bool)item);
        }

        protected override void OnItemUnsubscribed(Item item)
        {
            //ConsoleLogging.Get.Print("ON PLAYER ITEM UNSUBSSCRIBED: {0}, {1}", item.Id.Substring(0, 3), (ItemType)item.Type);
            base.OnItemUnsubscribed(item);
            var unsubscribeEvent = new ItemUnsubscribed { ItemId = item.Id, ItemType = item.Type, InterestAreaId = this.Id };
            var eventData = new EventData((byte)EventCode.ItemUnsubscribed, unsubscribeEvent);
            this.Peer.SendEvent(eventData, new SendParameters { ChannelId = Settings.ItemEventChannel });
        }
    }
}
