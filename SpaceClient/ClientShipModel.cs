

namespace Nebula.Client
{
    using Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ClientShipModel
    {
        public readonly ClientShipModelSlot es;
        public readonly ClientShipModelSlot cb;
        public readonly ClientShipModelSlot df;
        public readonly ClientShipModelSlot cm;
        public readonly ClientShipModelSlot dm;

        public readonly ClientShipModelSlot[] slots;

        public ClientShipModel() {
            es = new ClientShipModelSlot(ShipModelSlotType.ES);
            cb = new ClientShipModelSlot(ShipModelSlotType.CB);
            df = new ClientShipModelSlot(ShipModelSlotType.DF);
            cm = new ClientShipModelSlot(ShipModelSlotType.CM);
            dm = new ClientShipModelSlot(ShipModelSlotType.DM);

            slots = new ClientShipModelSlot[] { es, cb, df, cm, dm };
        }

        public ClientShipModelSlot Slot(ShipModelSlotType type) {
            switch (type) {
                case ShipModelSlotType.CB: return cb;
                case ShipModelSlotType.CM: return cm;
                case ShipModelSlotType.DF: return df;
                case ShipModelSlotType.DM: return dm;
                case ShipModelSlotType.ES: return es;
                default:
                    return null;
            }
        }

        public void Clear() {
            foreach (var slot in this.slots) {
                slot.Clear();
            }
        }

        public void SetModule(ClientShipModule module) {
            Slot(module.type).SetModule(module);
        }

        public void RemoveModule(ShipModelSlotType type) {
            Slot(type).RemoveModule();
        }

        public bool HasModule(ShipModelSlotType type) {
            return Slot(type).HasModule;
        }

        public ClientShipModule Module(ShipModelSlotType type) {
            return Slot(type).Module;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder();
            foreach (var s in slots) {
                sb.AppendLine(s.ToString());
            }
            return sb.ToString();
        }

        public bool HasAllModules()
        {
            return this.slots.All(s => s.HasModule);
        }


        public Dictionary<ShipModelSlotType, string> SlotPrefabs()
        {
            var prefabs = new Dictionary<ShipModelSlotType, string>();
            foreach(var slot in this.slots)
            {
                if(slot.HasModule && slot.Module.HasPrefab() )
                {
                    prefabs.Add(slot.Type, slot.Module.prefab);
                }
            }
            return prefabs;
        }
    }
}
