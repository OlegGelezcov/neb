using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;

namespace Nebula.Game.Events {
    public class KillBossEvent : BaseEvent {

        private bool mTargetDead = false;
        private string targetNpcId = "";
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected override bool CheckForComplete() {
            return mTargetDead;
        }

        protected override void OnActivated() {
            mTargetDead = false;
            //create target object
            var obj = ObjectCreate.EventNpc(nebulaObject.world as MmoWorld, this, "event_combat");
            targetNpcId = obj.Id;
            obj.AddToWorld();
        }

        protected override void OnDiactivated() {
            //create rewards for members
        }

        public void OnDeath(EventedObject obj) {
            log.Info("KillBossEvent.OnDeath() called");
            if (active) {
                if (obj.nebulaObject.Id == targetNpcId) {
                    mTargetDead = true;
                }
            }
        }

        public void OnReceiveDamage(EventMessage message) {
            if (active) {
                if (message.Source.nebulaObject.Id == targetNpcId) {
                    DamageInfo damageInfo = message.Data as DamageInfo;
                    if (damageInfo == null) {
                        return;
                    }
                    if (damageInfo.DamagerType != ItemType.Avatar) {
                        return;
                    }
                    NebulaObject damager;
                    nebulaObject.world.TryGetObject((byte)damageInfo.DamagerType, damageInfo.DamagerId, out damager);
                    if (damager) {
                        AddMember(damager);
                    }
                }
            }
        }


    }
}
