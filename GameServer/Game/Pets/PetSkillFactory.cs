using ExitGames.Logging;
using Nebula.Game.Utils;
using Space.Game;

namespace Nebula.Game.Pets {
    public class PetSkillFactory {
        private static ILogger s_Log = LogManager.GetCurrentClassLogger();

        public PetSkill Create(int skillId, IRes resource) {
            var skillData = resource.petSkills.GetSkill(skillId);
            if(skillData == null ) {
                s_Log.InfoFormat("pet skill = {0} not founded".Color(LogColor.orange), skillId);
                return null;
            }

            switch(skillData.id) {
                case 1:
                    return new AccelerationPetSkill(skillData);
                default:
                    return null;
            }
        }
    }
}
