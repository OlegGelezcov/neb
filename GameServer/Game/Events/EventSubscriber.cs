using Common;
using ExitGames.Logging;
using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Events {
    public abstract class EventSubscriber : NebulaBehaviour {

        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        public override int behaviourId {
            get {
                return (int)ComponentID.EventSubscriber;
            }
        }

        public override void Start() {
            base.Start();
            if(nebulaObject.mmoWorld().AddEventSubscriber(this)) {
                //s_Log.InfoFormat("successfully add self to event subscribers...".Lime());
            }
        }

        protected virtual void Disable() {
            if(nebulaObject.mmoWorld().RemoveEventSubscriber(this)) {
                //s_Log.InfoFormat("successfully removed self from event subscribers...".Lime());
            }
        }

        [ComponentMessage(false, Log = "EventSubscriber->Death() :orange")]
        public virtual void Death() {
            Disable();
        }

        public abstract bool OnEvent(BaseEvent evt);
    }
}
