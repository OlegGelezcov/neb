using Common;
using GameMath;
using System;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public abstract class ContractData {
        public string id { get; private set; }
        public ContractCategory category { get; private set; }
        public int minLevel { get; private set; }
        private ContractDataRewardCollection m_Rewards;

        public ContractData(XElement element ) {
            id = element.GetString("id");
            category = (ContractCategory)Enum.Parse(typeof(ContractCategory), element.GetString("category"));
            minLevel = element.GetInt("min_level");

            var rewardsElement = element.Element("rewards");
            m_Rewards = new ContractDataRewardCollection(rewardsElement);
        }

        public ContractDataRewardCollection rewards {
            get {
                return m_Rewards;
            }
        }


        public Race GenerateTargetRace(Race sourceRace) {
            switch (sourceRace) {
                case Race.Humans: {
                        if (Rand.Int() % 2 == 0) {
                            return Race.Borguzands;
                        } else {
                            return Race.Criptizoids;
                        }
                    }
                case Race.Borguzands: {
                        if (Rand.Int() % 2 == 0) {
                            return Race.Humans;
                        } else {
                            return Race.Criptizoids;
                        }
                    }
                case Race.Criptizoids: {
                        if (Rand.Int() % 2 == 0) {
                            return Race.Humans;
                        } else {
                            return Race.Borguzands;
                        }
                    }
                default: {
                        int val = Rand.Int() % 3;
                        if (val == 0) {
                            return Race.Humans;
                        } else if (val == 1) {
                            return Race.Borguzands;
                        } else {
                            return Race.Criptizoids;
                        }
                    }
            }
        }
    }
}
