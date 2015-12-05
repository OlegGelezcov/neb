// ClientItemEventInfo.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Friday, December 5, 2014 1:04:57 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//


namespace Nebula.Client {

    public class ClientItemEventInfo {
        public bool FromEvent { get; private set; }
        public string EventId { get; private set; }
        public string EventWorldId { get; private set; }

        public ClientItemEventInfo(bool fromEvent, string eventId, string eventWorldId) {
            this.FromEvent = fromEvent;
            this.EventId = eventId;
            this.EventWorldId = eventWorldId;
        }

        public static ClientItemEventInfo Default {
            get {
                return new ClientItemEventInfo(false, string.Empty, string.Empty);
            }
        }

        public void SetFromEvent(bool fromEvent) {
            this.FromEvent = fromEvent;
        }

        public void SetEventId(string eventId) {
            this.EventId = eventId;
        }

        public void SetEventWorldId(string eventWorldId) {
            this.EventWorldId = eventWorldId;
        }
    }
}
