using System;
using System.Collections;
using Common;
using ServerClientCommon;

namespace Space.Game.Ship
{
    public class ShipModelSlotBase : IInfoSource
    {
        protected ShipModule module;

        public void Replace(ShipModelSlotBase other) {
            module = other.module;
        }

        public bool SetModule(ShipModule module, out ShipModule prevModule)
        {
            prevModule = null;
            if (module != null) {
                if (module.SlotType == Type) {
                    if (RemoveModule(out prevModule)) {
                        this.module = module;
                        this.module.Bind();
                        if(prevModule != null) {
                            prevModule.Bind();
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public bool RemoveModule(out ShipModule oldModule)
        {
            oldModule = module;
            module = null;
            return true;
        }

        public virtual ShipModelSlotType Type
        {
            get { return ShipModelSlotType.CB; }
        }

        public bool HasModule
        {
            get { return module != null; }
        }

        public bool Empty {
            get {
                return module == null;
            }
        }

        

        public ShipModule Module
        {
            get { return module; }
        }

        public virtual Hashtable GetInfo() {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Type, Type.toByte());
            info.Add((int)SPC.Info, HasModule ? module.GetInfo() : new Hashtable());
            return info;
        }
    }
}
