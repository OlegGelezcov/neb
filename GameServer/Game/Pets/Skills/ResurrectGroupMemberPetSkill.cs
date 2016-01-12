using Common;
using Nebula.Engine;
using Nebula.Game.Pets.Conditions;
using Nebula.Pets;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Pets.Skills {
    public class ResurrectGroupMemberPetSkill : PetSkill {

        private NebulaObject m_LastMember;

        public ResurrectGroupMemberPetSkill(PetSkillInfo skill, NebulaObject source) 
            : base(skill, source) {
            if (data.prob < 1f) {
                AddCondition(new ProbCondition(data.prob, source));
            }
            if (data.cooldown > 0) {
                AddCondition(new CooldownCondition(data.cooldown, source));
            }
        }

        public override bool DoUse() {
            return true;
        }

        public override bool UseExplicit(NebulaObject target) {
            m_LastMember = target;
            return base.UseExplicit(target);
        }

        protected override Hashtable GetProperties() {
            Hashtable hash = base.GetProperties();
            if (m_LastMember) {
                hash.Add((int)SPC.Target, m_LastMember.Id);
                hash.Add((int)SPC.TargetType, m_LastMember.Type);
            } else {
                hash.Add((int)SPC.Target, string.Empty);
                hash.Add((int)SPC.TargetType, (byte)ItemType.Bot);
            }
            return hash;
        }
    }
}
