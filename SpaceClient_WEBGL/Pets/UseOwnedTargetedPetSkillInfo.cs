using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Pets {
    public class UseOwnedTargetedPetSkillInfo : UsePetSkillInfo {

        private string m_Target;
        private ItemType m_TargetType;

        public UseOwnedTargetedPetSkillInfo(Hashtable hash) : base(hash) {
            m_Target = hash.GetValueString((int)SPC.Target);
            m_TargetType = (ItemType)hash.GetValueByte((int)SPC.TargetType, (byte)ItemType.Bot);
        }

        public string targetId {
            get {
                return m_Target;
            }
        }

        public ItemType targetType {
            get {
                return m_TargetType;
            }
        }
    }
}
