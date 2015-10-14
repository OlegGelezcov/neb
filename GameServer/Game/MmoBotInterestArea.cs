using ExitGames.Concurrency.Fibers;
using Space.Server;
using Space.Server.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game
{
    public class MmoBotInterestArea : InterestArea 
    {
        private readonly Dictionary<Item, IDisposable> eventChannelSubscriptions;
        private readonly IFiber fiber;


        public MmoBotInterestArea(byte id, IWorld world, IFiber fiber ) : base(id, world )
        {
            this.fiber = fiber;
            this.eventChannelSubscriptions = new Dictionary<Item, IDisposable>();
        }

        protected override void OnItemSubscribed(ItemSnapshot itemSnapshot)
        {
            Item item = itemSnapshot.Source;

            // publish event messages
            IDisposable messageReceiver = item.EventChannel.Subscribe(this.fiber, this.SubscribedItem_OnItemEvent);
            this.eventChannelSubscriptions.Add(item, messageReceiver);
        }

        protected override void OnItemUnsubscribed(Item item)
        {
            //ConsoleLogging.Get.Print("item unsubscribed: {0}", item.Id.Substring(0, 3));
            IDisposable messageReceiver = this.eventChannelSubscriptions[item];
            this.eventChannelSubscriptions.Remove(item);
            messageReceiver.Dispose();
        }

        private void SubscribedItem_OnItemEvent(ItemEventMessage message)
        {
        }
    }
}
