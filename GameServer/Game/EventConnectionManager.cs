//// EventConnectionManager.cs
//// Nebula
//// 
//// Created by Oleg Zhelestcov on Thursday, December 4, 2014 1:38:19 PM
//// Copyright (c) 2014 KomarGames. All rights reserved.
////
//namespace Space.Game
//{
//    using Common;
//    using Space.Game.Events;
//    using System.Collections;
//    using System.Collections.Generic;

//    public class EventConnectionManager : IInfo
//    {
//        private Dictionary<string, Dictionary<string, WorldEvent>> eventConnections;

//        public EventConnectionManager()
//        {
//            this.eventConnections = new Dictionary<string, Dictionary<string, WorldEvent>>();
//        }

//        /// <summary>
//        /// Adds a new or update an existing event in the event list. Returns a flag indicating that the event is new or is already contained in the list of events
//        /// </summary>
//        public void AddEvent(WorldEvent e, out bool isNewEvent)
//        {
//            //ConsoleLogging.Get.Print(LogFilter.ALL, "actor add event callled");
//            Dictionary<string, WorldEvent> worldEvents;
//            if(this.eventConnections.TryGetValue(e.World.Name, out worldEvents))
//            {
//                if (worldEvents.ContainsKey(e.Id))
//                {
//                    worldEvents[e.Id] = e;
//                    isNewEvent = false;
//                }
//                else
//                {
//                    worldEvents.Add(e.Id, e);
//                    isNewEvent = true;
//                }
//            }
//            else
//            {
//                worldEvents = new Dictionary<string, WorldEvent> { { e.Id, e } };
//                this.eventConnections.Add(e.World.Name, worldEvents);
//                isNewEvent = true;
//            }
//        }

//        public void Remove(string world, string evt )
//        {
//            Dictionary<string, WorldEvent> worldEvents;
//            if(this.eventConnections.TryGetValue(world, out worldEvents))
//            {
//                worldEvents.Remove(evt);
//            }
//        }

//        public void Clear()
//        {
//            this.eventConnections.Clear();
//        }

//        public Hashtable GetInfo()
//        {
//            Hashtable totalResult = new Hashtable();
//            foreach(var worldEventsPair in this.eventConnections)
//            {
//                Hashtable eventsInfo = new Hashtable();
//                foreach(var ePair in worldEventsPair.Value)
//                {
//                    eventsInfo.Add(ePair.Key, ePair.Value.GetInfo());
//                }
//                totalResult.Add(worldEventsPair.Key, eventsInfo);
//            }
//            //ConsoleLogging.Get.Print(LogFilter.ALL, totalResult.ToStringBuilder().ToString());
//            return totalResult;
//        }

//        public void ParseInfo(Hashtable info)
//        {

//        }
//    }
//}
