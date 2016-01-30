/*
using System;
using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Events;
using Space.Game;

namespace Nebula.Game.Components {
    public class EventedObject : NebulaBehaviour {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public BaseEvent ownedEvent { get; private set; }

        public void SetOwnedEvent(BaseEvent inOwnedEvent) {
            ownedEvent = inOwnedEvent;
        } 

        public override void Start() {
            
        }

        public override void Update(float deltaTime) {

        }


        public void Death() {
            
            log.Info("call Death() message on evented object");
            if(ownedEvent) {
                ownedEvent.nebulaObject.SendMessage(ComponentMessages.OnDeath, this);
            }
        }

        public void ReceiveDamage(DamageInfo damageInfo) {
            log.Info("call ReceiveDamage on evented object");
            if(ownedEvent) {
                ownedEvent.nebulaObject.SendMessage(ComponentMessages.OnReceiveDamage, new EventMessage {  Source = this, Data = damageInfo  } );
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.EventedObject;
            }
        }
    }
}
*/