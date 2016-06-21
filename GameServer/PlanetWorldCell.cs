using System;
using System.Collections;
using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components.PlanetObjects;
using Nebula.Resources.Zones.Planets;
using ServerClientCommon;

namespace Nebula {

    public class PlanetWorldCell : IInfoSource {
        private PlanetCellData m_Data;
        private PlanetObjectBase m_CellObject;

        public PlanetWorldCell() { }

        public void SetData(PlanetCellData d) {
            m_Data = d;
        }

        public void SetObject(PlanetObjectBase obj) {
            m_CellObject = obj;
        }

        public Hashtable GetInfo() {
            Hashtable hash = new Hashtable();
            if(m_Data != null ) {
                hash.Add((int)SPC.Row, m_Data.row);
                hash.Add((int)SPC.Column, m_Data.column);
                hash.Add((int)SPC.Position, m_Data.position.ToArray());
            }
            hash.Add((int)SPC.HasCellObject, hasCellObject);
            if (hasCellObject ) {
                hash.Add((int)SPC.ItemId, cellObject.nebulaObject.Id);
                hash.Add((int)SPC.ItemType, cellObject.nebulaObject.Type);
            }
            return hash;
        }

        public PlanetCellData data {
            get {
                return m_Data;
            }
        }

        public Vector3 position {
            get {
                if(data != null ) {
                    return data.position;
                }
                return Vector3.Zero;
            }
        }

        public PlanetObjectBase cellObject {
            get {
                return m_CellObject;
            }
        }

        public bool hasCellObject {
            get {
                if(m_CellObject != null ) {
                    if(m_CellObject.nebulaObject ) {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsObject(string objId ) {
            if(m_CellObject != null && m_CellObject.nebulaObject != null ) {
                return m_CellObject.nebulaObject.Id == objId;
            }
            return false;
        }

    }
}
