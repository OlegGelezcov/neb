using Common;
using ServerClientCommon;
using System.Collections;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public abstract class CombatBaseAIComponentData : BaseAIComponentData {

        public bool useHitProbForAgro { get; private set; } = false;

        public CombatBaseAIComponentData(XElement e) : base(e) {
            if(e.HasAttribute("use_hit_prob_for_agro")) {
                useHitProbForAgro = e.GetBool("use_hit_prob_for_agro");
            }
        }

        public CombatBaseAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, bool useHitProbForAgro)
            : base(inAlignWithForwardDirection, inRotationSpeed ) {
            this.useHitProbForAgro = useHitProbForAgro;
        }
        public CombatBaseAIComponentData(Hashtable hash) : base(hash) {
            useHitProbForAgro = hash.GetValue<bool>((int)SPC.UseHitProbForAgro, false);
        }
    }
}
