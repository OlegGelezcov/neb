using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public abstract class CombatBaseAIComponentData : BaseAIComponentData {

        public bool useHitProbForAgro { get; private set; } = false;
#if UP
        public CombatBaseAIComponentData(UPXElement e) : base(e) {
            if (e.HasAttribute("use_hit_prob_for_agro")) {
                useHitProbForAgro = e.GetBool("use_hit_prob_for_agro");
            }
        }
#else
        public CombatBaseAIComponentData(XElement e) : base(e) {
            if(e.HasAttribute("use_hit_prob_for_agro")) {
                useHitProbForAgro = e.GetBool("use_hit_prob_for_agro");
            }
        }
#endif
        public CombatBaseAIComponentData(bool inAlignWithForwardDirection, float inRotationSpeed, bool useHitProbForAgro)
            : base(inAlignWithForwardDirection, inRotationSpeed ) {
            this.useHitProbForAgro = useHitProbForAgro;
        }
        public CombatBaseAIComponentData(Hashtable hash) : base(hash) {
            useHitProbForAgro = hash.GetValue<bool>((int)SPC.UseHitProbForAgro, false);
        }
    }
}
