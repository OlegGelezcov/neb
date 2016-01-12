using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Pets {
    public class UsePetSkillInfo {
        private string m_Source;
        private ItemType m_SourceType;
        private int m_SkillId;

        public UsePetSkillInfo(Hashtable hash) {
            m_SkillId = hash.GetValueInt((int)SPC.Skill, -1);
            m_SourceType = (ItemType)hash.GetValueByte((int)SPC.SourceType, (byte)ItemType.Bot);
            m_Source = hash.GetValueString((int)SPC.Source, string.Empty);
        }

        public string sourceId {
            get {
                return m_Source;
            }
        }

        public ItemType sourceType {
            get {
                return m_SourceType;
            }
        }

        public int skillId {
            get {
                return m_SkillId;
            }
        }
    }
}
