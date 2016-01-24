using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class FounderCubeInventoryObjectInfo : IInventoryObjectInfo {

        private bool m_Binded;
        private string m_Id;
        private Hashtable m_Raw;
        private float m_UseTime;

        public FounderCubeInventoryObjectInfo(Hashtable hash) {
            m_Raw = hash;
            ParseInfo(hash);
        }

        #region IInventoryObjectInfo interface
        public bool binded {
            get {
                return m_Binded;
            }
        }

        public string Id {
            get {
                return m_Id;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                return m_Raw;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.founder_cube;
            }
        }

        public Hashtable GetInfo() {
            return rawHash;
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }

        public float useTime {
            get {
                return m_UseTime;
            }
        }

        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            m_Id = info.GetValueString((int)SPC.Id);
            m_Binded = info.GetValueBool((int)SPC.Binded);
            m_UseTime = info.GetValueFloat((int)SPC.UseTime);
        } 
        #endregion

        public bool readyToUse {
            get {
                return (CommonUtils.SecondsFrom1970() - useTime) > CommonUtils.ONE_MONTH;
            }
        }
    }
}
