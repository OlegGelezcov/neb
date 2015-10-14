using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using Space.Game;
using Space.Game.Events;
using Space.Game.Resources;
using Space.Server;
using System;
using System.Collections;
using System.Collections.Concurrent;

namespace Nebula.Game.Events {

    /// <summary>
    /// Represent base event object in world
    /// </summary>
    public abstract class BaseEvent : NebulaBehaviour, IInfoSource {

        public WorldEventData data { get; private set; }

        public ConcurrentDictionary<string, string> membeers { get; private set; }

        public bool active { get; private set; }

        protected float timer { get; set; }

        public override void Awake() {
            membeers = new ConcurrentDictionary<string, string>();
        }

        public override void Start() {
            if(data == null) {
                throw new Exception("Data of event must be setted before Start()");
            }
            timer = data.Cooldown;
            SetActive(false);
        }

        public override void Update(float deltaTime) {
            if(!active) {
                timer -= deltaTime;
                if(timer <= 0f ) {
                    membeers.Clear();
                    SetActive(true);
                    SendActiveToNearPlayers();             
                    OnActivated();
                }
            } else {
                if(CheckForComplete()) {
                    SetActive(false);
                    RewardExpNearestMembers();
                    timer = data.Cooldown;
                    OnDiactivated();
                }
            }
        }

        private void SendActiveToNearPlayers() {
            var nearestPlayers = (nebulaObject.world as MmoWorld).GetItems((item) => {
                if (item.Type == (byte)ItemType.Avatar && nebulaObject.transform.DistanceTo(item.transform) <= data.Radius) {
                    return true;
                }
                return false;
            });
            foreach(var playerPair in nearestPlayers) {
                playerPair.Value.GetComponent<MmoMessageComponent>().ReceiveGameEvent(nebulaObject.Id, active);
            }
        }

        public void RewardExpNearestMembers() {
            foreach(var memberPair in membeers) {
                Item player;
                if((nebulaObject.world as MmoWorld).ItemCache.TryGetItem((byte)ItemType.Avatar, memberPair.Key, out player)) {
                    player.GetComponent<PlayerCharacterObject>().AddExp(50);
                    player.GetComponent<MmoMessageComponent>().ReceiveGameEvent(nebulaObject.Id, active);
                }
            }
        }


        public void AddMember(NebulaObject obj) {
            MmoActor actor = obj.GetComponent<MmoActor>();
            if(actor ) {
               if(!membeers.ContainsKey(obj.Id)) {
                    membeers.TryAdd(obj.Id, obj.Id);
                }
            } 
        }

        public void SetEventData(WorldEventData data) {
            this.data = data;
        } 

        public void SetActive(bool inActive) {
            active = inActive;
            props.SetProperty((byte)PS.Event, GetInfo());
        }

        protected abstract void OnActivated();
        protected abstract void OnDiactivated();
        protected abstract bool CheckForComplete();

        public Hashtable GetInfo() {
            return new Hashtable { { (int)SPC.Id, data.Id }, { (int)SPC.Active, active } };
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Event;
            }
        }
    }

    public class EventMessage {
        public EventedObject Source;
        public object Data;
    }
}
