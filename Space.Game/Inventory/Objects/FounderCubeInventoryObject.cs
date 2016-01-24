using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System;
using System.Collections;

namespace Nebula.Inventory.Objects {

    /// <summary>
    /// Object what represent founder cube
    /// </summary>
    public class FounderCubeInventoryObject : IInventoryObject {

        private Hashtable m_Raw;
        private float m_UseTime;


        public FounderCubeInventoryObject() {
            Id = Guid.NewGuid().ToString();
            m_UseTime = 0;
        }

        public FounderCubeInventoryObject(Hashtable hash) {
            ParseInfo(hash);
        }

        #region IInventoryObject interface

        /// <summary>
        /// Binded or not to character ?
        /// </summary>
        public bool binded {
            get {
                return true;
            }
        } 

        /// <summary>
        /// Id of object
        /// </summary>
        public string Id {
            get;
            private set;
        }

        /// <summary>
        /// Level not applicable ( always zero )
        /// </summary>
        public int Level {
            get;
            private set;
        } = 0;

        /// <summary>
        /// For compatibility with old code (always Inventory)
        /// </summary>
        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        /// <summary>
        /// Raw dictionary for this item
        /// </summary>
        public Hashtable rawHash {
            get {
                if(m_Raw == null ) {
                    m_Raw = GetInfo();
                }
                return m_Raw;
            }
        }

        /// <summary>
        /// Not splittable
        /// </summary>
        public bool splittable {
            get {
                return false;
            }
        }

        /// <summary>
        /// Inventory type of object
        /// </summary>
        public InventoryObjectType Type {
            get {
                return InventoryObjectType.founder_cube;
            }
        }

        /// <summary>
        /// Make this object binded
        /// </summary>
        public void Bind() {
        }

        /// <summary>
        /// Get dictionary for network transfer this object
        /// </summary>
        /// <returns></returns>
        public Hashtable GetInfo() {
            m_Raw = InventoryUtils.ItemHash(Id, Level, ObjectColor.white, Type, (PlacingType)placingType, binded, splittable);
            m_Raw.Add((int)SPC.UseTime, m_UseTime);

            return m_Raw;
        }

        /// <summary>
        /// Parse object from network dictionary
        /// </summary>
        /// <param name="info"></param>
        public void ParseInfo(Hashtable info) {
            m_Raw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_UseTime = info.GetValue<float>((int)SPC.UseTime, 0f);
        } 
        #endregion

        public float useTime {
            get {
                return m_UseTime;
            }
        }

        public bool ReadyToUse() {
            return (CommonUtils.SecondsFrom1970() - useTime) > CommonUtils.ONE_MONTH;
        }

        public void SetUseTimeNow() {
            m_UseTime = CommonUtils.SecondsFrom1970();
        }
    }
}
