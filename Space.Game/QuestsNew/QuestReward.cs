using Common;
using System.Xml.Linq;

namespace Nebula.Quests {

    public class QuestReward {
        public QuestRewardType Type { get; private set; }
        public int Count { get; private set; }
        

        public virtual void Load(XElement element) {
            Type = element.GetEnum<QuestRewardType>("type");
            Count = element.GetInt("count", 1);
        }

        public override string ToString() {
            return $"Reward => {Type}, Count => {Count}";
        }
    }


    public class InventoryItemQuestReward : QuestReward {
        public InventoryObjectType ObjectType { get; private set; }

        public override void Load(XElement element) {
            base.Load(element);
            if (element.HasAttribute("inventory_type")) {
                ObjectType = element.GetEnum<InventoryObjectType>("inventory_type");
            }
        }

        public override string ToString() {
            return $"{base.ToString()}, Inventory Object Type: {ObjectType}";
        }
    }

    public class MaterialItemQuestReward : InventoryItemQuestReward {

        public string OreId { get; private set; }

        public override void Load(XElement element) {
            base.Load(element);
            OreId = element.GetString("ore_id");
        }

        public override string ToString() {
            return $"{base.ToString()}, Ore Id: {OreId}";
        }
    }

    public class SchemeItemQuestReward : InventoryItemQuestReward {
        public ShipModelSlotType Slot { get; private set; }
        public ObjectColor Color { get; private set; }

        public override void Load(XElement element) {
            base.Load(element);
            Slot = element.GetEnum<ShipModelSlotType>("slot");
            Color = element.GetEnum<ObjectColor>("color");
        }

        public override string ToString() {
            return $"{base.ToString()}, Slot => {Slot}, Color => {Color}";
        }
    }

    public class WeaponItemQuestReward : InventoryItemQuestReward {
        public ObjectColor Color { get; private set; }

        public override void Load(XElement element) {
            base.Load(element);
            Color = element.GetEnum<ObjectColor>("color");
        }

        public override string ToString() {
            return $"{base.ToString()}, Color => {Color}";
        }
    }

    public interface IQuestRewardContext {

    }
}
