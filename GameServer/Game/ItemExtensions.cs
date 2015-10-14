// ItemExtensions.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Wednesday, December 10, 2014 4:18:48 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//
namespace Space.Game
{
    using Common;
    using Photon.SocketServer;
    using Space.Server;
    using Space.Server.Events;
    using Space.Server.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class ItemExtensions
    {
        public static void SendEventShipDestroyed(this Item item, bool shipDestroyed )
        {
            if (item == null)
                return;
            if (item.Disposed)
                return;
            if (item.Fiber == null)
                return;

            //CL.Out(LogFilter.NPC, "SEND ITEM SHIP DESTROYED");

            item.Fiber.Enqueue(() =>
                {
                    if (item.Disposed)
                    {
                        CL.Out(LogFilter.NPC, "ITEM DISPOSED, SENDING NOT ALLOWED");
                        return;
                    }

                    var eventInstance = new ItemGeneric
                    {
                        ItemId = item.Id,
                        ItemType = item.Type,
                        CustomEventCode = CustomEventCode.ItemShipDestroyed.toByte(),
                        EventData = shipDestroyed
                    };
                    var eventData = new EventData(EventCode.ItemGeneric.toByte(), eventInstance);
                    SendParameters sendParameters = new SendParameters
                    {
                        Unreliable = false,
                        ChannelId = Settings.ItemEventChannel,
                    };

                    if(item is IMmoItem)
                    {
                        (item as IMmoItem).ReceiveEvent(eventData, sendParameters);
                    }

                    item.EventChannel.Publish(new ItemEventMessage(item, eventData, sendParameters));

                });
        }
    }
}
