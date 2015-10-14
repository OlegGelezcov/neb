using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Nebula.Client
{
    public class ClientShipModelSlot
    {
        private ShipModelSlotType type;
        private ClientShipModule module;

        public ClientShipModelSlot(ShipModelSlotType type) {
            this.type = type;
        }

        public void SetModule(ClientShipModule module) {
            this.module = module;
        }

        public void RemoveModule() {
            this.module = null;
        }

        public ShipModelSlotType Type {
            get {
                return type;
            }
        }

        public bool HasModule {
            get {
                return (module != null);
            }
        }

        public bool Empty {
            get {
                return (this.module == null);
            }
        }

        public ClientShipModule Module {
            get {
                return module;
            }
        }

        public void Clear() { SetModule(null); }

        public override string ToString()
        {
            string result = string.Format("Slot: {0}\n", Type);
            result += (module != null) ? module.ToString() : "NONE";
            return result;
        }
    }
}
