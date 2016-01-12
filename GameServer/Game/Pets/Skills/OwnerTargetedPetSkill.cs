using Nebula.Engine;
using Nebula.Pets;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Pets.Skills {
    public abstract class OwnerTargetedPetSkill : PetSkill {

        public OwnerTargetedPetSkill(PetSkillInfo skillInfo, NebulaObject source)
            : base(skillInfo, source) { }

        protected override Hashtable GetProperties() {
            Hashtable hash = base.GetProperties();
            if (pet.owner) {
                hash.Add((int)SPC.Target, pet.owner.Id);
                hash.Add((int)SPC.TargetType, pet.owner.Type);
            }
            return hash;
        }
    }
}
