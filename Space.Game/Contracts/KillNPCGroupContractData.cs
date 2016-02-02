using Common;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class KillNPCGroupContractData : ContractData  {

        private NPCGroupDataCollection m_Groups;
        private ContractDataRewardCollection m_Rewards;

        public KillNPCGroupContractData(XElement element) 
            : base(element) {
            var groupsElement = element.Element("groups");
            m_Groups = new NPCGroupDataCollection(groupsElement);

            var rewardsElement = element.Element("rewards");
            m_Rewards = new ContractDataRewardCollection(rewardsElement);
        }

        public int GetGroupCount(Race race) {
            return m_Groups.GetGroupCount(race);
        }

        public int GetGroupCount(Race race, int level ) {
            return m_Groups.GetGroupCount(race, level);
        }

        public bool HasGroups(Race race, int level ) {
            return m_Groups.GetGroupCount(race, level) > 0;
        }

        public NPCGroupData GetRandomGroup(Race race, int level) {
            return m_Groups.GetRandomGroup(race, level);
        }
    }
}
