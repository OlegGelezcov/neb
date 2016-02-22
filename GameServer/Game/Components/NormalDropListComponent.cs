using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {
    public class NormalDropListComponent : DropListComponent {
        public override void Init(DropListComponentData data) {
            base.Init(data);
        }

        public override ActorDropListPair GetDropList(DamageInfo actor) {
            return new ActorDropListPair(actor, dropList);
        }
    }
}
