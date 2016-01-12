using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Utils;
using Space.Game;

namespace Nebula.Game.Pets.Skills {
    public class PetSkillFactory {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        public PetSkill Create(int skillId, IRes resource, NebulaObject source) {
            var skillData = resource.petSkills.GetSkill(skillId);
            if(skillData == null ) {
                s_Log.InfoFormat("pet skill = {0} not founded".Color(LogColor.orange), skillId);
                return null;
            }

            switch(skillData.id) {
                case 1:
                    return new AccelerationPetSkill(skillData, source);
                case 2:
                    return new DecreaseInputDamageSkill(skillData, source);
                case 3:
                    return new DecreasOwnerSubscriberDamageSkill(skillData, source);
                case 4:
                case 5:
                    return new ReflectionDamageSkill(skillData, source);
                case 6:
                    return new RestoreHpPetSkill(skillData, source);
                case 7:
                    return new AdditionalDamagePetSkill(skillData, source);
                case 8:
                    return new AbsorbDamagePetSkill(skillData, source);
                case 9:
                    return new IncreaseEnergyCostPetSkill(skillData, source);
                case 10:
                    return new VampirismPetSkill(skillData, source);
                case 11:
                    return new ResurrectGroupMemberPetSkill(skillData, source);
                default:
                    return null;
            }
        }
    }
}
