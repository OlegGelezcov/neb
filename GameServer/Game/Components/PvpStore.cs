using Common;
using Nebula.Engine;
using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class PvpStore : NebulaBehaviour {

        public void Init(PvpStoreComponentData data) { }

        public override int behaviourId {
            get {
                return (int)ComponentID.PvpStore;
            }
        }
    }
}
