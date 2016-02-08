using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nebula.Game.Events {
    public class LocationTrigger : NebulaBehaviour {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public class TriggerSubscriber {
            private NebulaObject m_Object;
            private float m_Timer;

            public TriggerSubscriber(NebulaObject obj, float timer ) {
                m_Object = obj;
                m_Timer = timer;
            }

            public float timer {
                get {
                    return m_Timer;
                }
            }

            public string id {
                get {
                    if(m_Object != null ) {
                        return m_Object.Id;
                    }
                    return string.Empty;
                }
            }

            public NebulaObject nebulaObject {
                get {
                    return m_Object;
                }
            }

            public void AddTime(float deltaTime) {
                m_Timer += deltaTime;
            }

           

        }


        private const float kHandleInterval = 5;
        private LocationTriggerComponentData m_Data;
        private readonly ConcurrentDictionary<string, TriggerSubscriber> m_Subscribers = new ConcurrentDictionary<string, TriggerSubscriber>();
        private readonly List<string> m_RemovedIds = new List<string>();
        private float m_HandleTimer = 0f;

        public void Init(LocationTriggerComponentData data) {
            m_Data = data;
        }

        public float radius {
            get {
                if(m_Data != null ) {
                    return m_Data.radius;
                }
                return 0f;
            }
        }

        public string triggerName {
            get {
                if(m_Data != null ) {
                    return m_Data.name;
                }
                return string.Empty;
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            if(m_Data == null ) { return; }

            m_HandleTimer += deltaTime;
            if (m_HandleTimer >= kHandleInterval) {
                HandleDestroyed();
                HandleExitTrigger();
                HandleTriggerStay(m_HandleTimer);
                HandleTriggerEnter();
                m_HandleTimer = 0.0f;
            }
        }

        private void HandleDestroyed() {
            if (m_RemovedIds.Count > 0) {
                m_RemovedIds.Clear();
            }

            foreach(var pts in m_Subscribers) {
                if(false == pts.Value.nebulaObject) {
                    m_RemovedIds.Add(pts.Key);
                }
            }

            TriggerSubscriber ts;
            foreach(string id in m_RemovedIds) {
                if(Unsubscribe(id, out ts)) {
                    s_Log.InfoFormat("remove not valid trigger: {0}", id);
                }
            }
        }

        private void HandleExitTrigger() {
            if (m_RemovedIds.Count > 0) {
                m_RemovedIds.Clear();
            }
            foreach(var pts in m_Subscribers) {
                if(pts.Value.nebulaObject) {
                    float distance = transform.DistanceTo(pts.Value.nebulaObject.transform);
                    if(distance > m_Data.radius ) {
                        m_RemovedIds.Add(pts.Key);
                    }
                }
            }

            if(m_RemovedIds.Count > 0) {
                foreach(string id in m_RemovedIds ) {
                    TriggerSubscriber ts;
                    if(Unsubscribe(id, out ts)) {
                        if(ts.nebulaObject) {
                            nebulaObject.mmoWorld().OnEvent(new TriggerEvent(ts.timer, ts.nebulaObject, EventType.TriggerExit, nebulaObject));
                        }
                    }
                }
            }
        }

        private void HandleTriggerEnter() {
            var inTriggerItems = nebulaObject.mmoWorld().GetItems(ItemType.Avatar, (it) => {
                if (transform.DistanceTo(it.transform) <= m_Data.radius) {
                    return true;
                }
                return false;
            });

            foreach(var pit in inTriggerItems) {
                if(!m_Subscribers.ContainsKey(pit.Key)) {
                    TriggerSubscriber subsriber;
                    if(Subscribe(pit.Value, out subsriber)) {
                        nebulaObject.mmoWorld().OnEvent(new TriggerEvent(subsriber.timer, subsriber.nebulaObject, EventType.TriggerEnter, nebulaObject));
                    }
                }
            }
        }

        private void HandleTriggerStay(float interval) {
            foreach(var pac in m_Subscribers) {
                pac.Value.AddTime(interval);
                if(pac.Value.nebulaObject) {
                    nebulaObject.mmoWorld().OnEvent(new TriggerEvent(pac.Value.timer, pac.Value.nebulaObject, EventType.TriggerStay, nebulaObject));
                }
            }
        }

        private bool Unsubscribe(string id, out TriggerSubscriber obj) {
            if(m_Subscribers.TryRemove(id, out obj)) {
                return true;
            }
            return false;
        }

        private bool Subscribe(NebulaObject obj, out TriggerSubscriber subscriber) {
            if(!m_Subscribers.ContainsKey(obj.Id)) {
                subscriber = new TriggerSubscriber(obj, 0);
                if(m_Subscribers.TryAdd(obj.Id, subscriber)) {
                    return true;
                }
            }
            subscriber = null;
            return false;
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Trigger;
            }
        }
    }
}
