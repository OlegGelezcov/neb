/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;

namespace Nebula.Client
{
    public class ClientWorldEventConnection : IInfo
    {
        private Dictionary<string, Dictionary<string, ClientWorldEventInfo>> events;

        public ClientWorldEventConnection()
        {
            this.events = new Dictionary<string,Dictionary<string,ClientWorldEventInfo>>();
        }

        public ClientWorldEventConnection(Hashtable info)
        {
            this.events = new Dictionary<string,Dictionary<string,ClientWorldEventInfo>>();
            this.ParseInfo(info);
        }



        public Hashtable GetInfo()
        {
            return new Hashtable();
        }

        public void ParseInfo(Hashtable info)
        {
            if (this.events == null)
                this.events = new Dictionary<string, Dictionary<string, ClientWorldEventInfo>>();
            this.events.Clear();

            foreach(DictionaryEntry topEntry in info )
            {
                Dictionary<string, ClientWorldEventInfo> worldEvents = new Dictionary<string, ClientWorldEventInfo>();
                Hashtable evtsInfo = topEntry.Value as Hashtable;
                foreach(DictionaryEntry inPair in evtsInfo )
                {
                    string evtId = inPair.Key.ToString();
                    Hashtable evtInf = inPair.Value as Hashtable;
                    worldEvents.Add(evtId, new ClientWorldEventInfo(evtInf));
                }
                this.events.Add(topEntry.Key.ToString(), worldEvents);
            }
        }

        public bool Contains(string worldId, string eventId )
        {
            if(this.events.ContainsKey(worldId))
            {
                if(this.events[worldId].ContainsKey(eventId))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsAnyEvents(string worldId )
        {
            if(this.events.ContainsKey(worldId))
            {
                return this.events[worldId].Count > 0;
            }
            return false;
        }

        public Dictionary<string, Dictionary<string, ClientWorldEventInfo>> Events
        {
            get
            {
                return this.events;
            }
        }

        public List<ClientWorldEventInfo> ActiveEvents()
        {
            List<ClientWorldEventInfo> result = new List<ClientWorldEventInfo>();
            foreach(var pWorldEvents in this.Events)
            {
                foreach(var pEvent in pWorldEvents.Value)
                {
                    if(pEvent.Value.Active)
                    {
                        result.Add(pEvent.Value);
                    }
                }
            }
            return result;
        }

        public Dictionary<string, ClientWorldEventInfo> EventsForWorld(string worldId )
        {
            Dictionary<string, ClientWorldEventInfo> resultEvents;
            if (this.events.TryGetValue(worldId, out resultEvents))
                return resultEvents;
            resultEvents = new Dictionary<string, ClientWorldEventInfo>();
            return resultEvents;
        }

        public void SetEvent(Hashtable info )
        {
            ClientWorldEventInfo evt = new ClientWorldEventInfo(info);
            Dictionary<string, ClientWorldEventInfo> filteredEvents;
            if(this.events.TryGetValue(evt.WorldId, out filteredEvents))
            {
                filteredEvents[evt.Id] = evt;
            }
            else
            {
                filteredEvents = new Dictionary<string, ClientWorldEventInfo> { { evt.Id, evt } };
                this.events.Add(evt.WorldId, filteredEvents);
            }
        }

        public ClientWorldEventInfo GetEvent(string worldId, string eventId )
        {
            Dictionary<string, ClientWorldEventInfo> filteredEvents;
            if(this.events.TryGetValue(worldId, out filteredEvents))
            {
                ClientWorldEventInfo result;
                if (filteredEvents.TryGetValue(eventId, out result))
                    return result;
            }
            return null;
        }
    }
}
*/
