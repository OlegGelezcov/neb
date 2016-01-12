using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Pets {
    public class PetSkillInfoFactory {
        public UsePetSkillInfo Create(Hashtable hash) {
            int skillId = hash.GetValueInt((int)SPC.Skill);
            switch (skillId) {
                case 1:
                    return new UseOwnedTargetedPetSkillInfo(hash);
                case 2:
                    goto case 1;
                case 3:
                    goto case 1;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    goto case 1;
                default:
                    return null;
            }
        }
    }
}
