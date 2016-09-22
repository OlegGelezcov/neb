using ExitGames.Logging;
using Nebula.Game.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Events {
    public class EventManager {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private const float kUpdateManagerInterval = 10.0f;

        private readonly ConcurrentDictionary<string, EventSubscriber> m_Subscribers = new ConcurrentDictionary<string, EventSubscriber>();
        private readonly List<string> m_InvalidSubscriberIds = new List<string>();
        private float m_UpdateManagerTimer = kUpdateManagerInterval;

        public bool Subscribe(EventSubscriber subscriber ) {
            bool oldRemovedSuccessfully = true;
            if(m_Subscribers.ContainsKey(subscriber.nebulaObject.Id)) {
                EventSubscriber removedSubscriber;
                if(false == m_Subscribers.TryRemove(subscriber.nebulaObject.Id, out removedSubscriber)) {
                    oldRemovedSuccessfully = false;
                }
            }

            bool success = false;
            if(oldRemovedSuccessfully) {
                success =  m_Subscribers.TryAdd(subscriber.nebulaObject.Id, subscriber);
            }

            //s_Log.InfoFormat("Object: {0} subscribed to events: {1}".Color(LogColor.cyan), subscriber.nebulaObject.Id, success);
            return success;
        }

        public bool Unsubscribe(EventSubscriber subscriber) {
            return Unsubscribe(subscriber.nebulaObject.Id);
        }

        private bool Unsubscribe(string subscriberId ) {
            bool success = false;
            if (m_Subscribers.ContainsKey(subscriberId)) {
                EventSubscriber removedSubscriber;
                success = m_Subscribers.TryRemove(subscriberId, out removedSubscriber);
            }

            //s_Log.InfoFormat("Object: {0} unsubscribed from events: {1}".Color(LogColor.cyan), subscriberId, success);
            return success;
        }

        public void OnEvent(BaseEvent evt) {
            foreach(var pSubscriber in m_Subscribers) {
                if(pSubscriber.Value) {
                    if(pSubscriber.Value.OnEvent(evt)) {
                        s_Log.InfoFormat("subscriber {0} handle event {1}".Color(LogColor.cyan), pSubscriber.Key, evt.eventType);
                    }
                }
            }
        }

        public void Update(float deltaTime) {

            m_UpdateManagerTimer -= deltaTime;
            if(deltaTime <= 0.0f ) {
                m_UpdateManagerTimer = kUpdateManagerInterval;
                m_InvalidSubscriberIds.Clear();

                foreach(var pSubscriber in m_Subscribers) {
                    if(false == pSubscriber.Value) {
                        m_InvalidSubscriberIds.Add(pSubscriber.Key);
                    }
                }

                if(m_InvalidSubscriberIds.Count >  0 ) {
                    foreach(var id in m_InvalidSubscriberIds) {
                        Unsubscribe(id);
                    }
                }
            }
        }
    }
}
