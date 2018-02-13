using Common;
using Nebula.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Quests {
    public class QuestRewardCollection {
        public List<QuestReward> Rewards { get; } = new List<QuestReward>();

        public void Load(XElement element) {
            Rewards.Clear();
            foreach(var rewardElement in element.Elements("reward")) {
                QuestReward reward = LoadReward(rewardElement);
                if (reward != null) {
                    Rewards.Add(reward);
                }
            }
        }

        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            Rewards.ForEach(r => {
                stringBuilder.AppendLine(r.ToString());
            });
            return stringBuilder.ToString();
        }

        private QuestReward LoadReward(XElement element) {
            QuestRewardType rewardType = element.GetEnum<QuestRewardType>("type");
            switch (rewardType) {
                case QuestRewardType.credits: {
                        QuestReward creditsReward = new QuestReward();
                        creditsReward.Load(element);
                        return creditsReward;
                    }
                case QuestRewardType.exp: {
                        QuestReward expReward = new QuestReward();
                        expReward.Load(element);
                        return expReward;
                    }
                case QuestRewardType.nebula_credits: {
                        QuestReward nebCreditsReward = new QuestReward();
                        nebCreditsReward.Load(element);
                        return nebCreditsReward;
                    }
                case QuestRewardType.item: {
                        InventoryObjectType inventoryObjectType = element.GetEnum<InventoryObjectType>("inventory_type");
                        switch(inventoryObjectType) {
                            case InventoryObjectType.Scheme: {
                                    SchemeItemQuestReward schemeReward = new SchemeItemQuestReward();
                                    schemeReward.Load(element);
                                    return schemeReward;
                                }
                            case InventoryObjectType.Material: {
                                    MaterialItemQuestReward materialReward = new MaterialItemQuestReward();
                                    materialReward.Load(element);
                                    return materialReward;
                                }
                            case InventoryObjectType.Weapon: {
                                    WeaponItemQuestReward weaponReward = new WeaponItemQuestReward();
                                    weaponReward.Load(element);
                                    return weaponReward;
                                }
                        }
                        break;
                    }
            }
            return null;
        }
    }
}
