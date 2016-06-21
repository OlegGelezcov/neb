using System;
using System.Collections;
using Common;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components.PlanetObjects;
using Space.Game;
using System.Collections.Generic;
using ServerClientCommon;

namespace Nebula {
    public class PlanetWorldCellBoard : IInfoSource {

        private MmoWorld m_World;
        private PlanetWorldCell[,] m_Cells;
        

        public PlanetWorldCellBoard() { }

        public void Setup(MmoWorld world ) {
            m_World = world;
            if(m_World.Zone.worldType == Common.WorldType.instance ) {
                m_Cells = new PlanetWorldCell[m_World.Zone.celss.rows, m_World.Zone.celss.columns];

                for(int i = 0; i < m_World.Zone.celss.rows; i++ ) {
                    for(int j = 0; j < m_World.Zone.celss.columns; j++ ) {
                        m_Cells[i, j] = new PlanetWorldCell();
                        m_Cells[i, j].SetData(m_World.Zone.celss.GetCell(i, j));
                    }
                }

            } else {
                m_Cells = new PlanetWorldCell[0, 0];
            }
        }

        public Vector3 GetCellPosition(int row, int column ) {
            if( row < m_World.Zone.celss.rows && column < m_World.Zone.celss.columns ) {
                return m_Cells[row, column].position;
            }
            return Vector3.Zero;
        }

        public bool HasCellObject(int row, int column ) {
            if(row < m_World.Zone.celss.rows && column < m_World.Zone.celss.columns ) {
                return m_Cells[row, column].hasCellObject;
            }
            return false;
        }

        public bool IsObjectAtCell(int row, int col, string objId ) {
            if (row < m_World.Zone.celss.rows && col < m_World.Zone.celss.columns) {
                return m_Cells[row, col].IsObject(objId);
            }
            return false;
        }

        public bool SetCellObject(int row, int column, PlanetObjectBase obj ) {
            if(m_World != null) {
                if(row < m_World.Zone.celss.rows &&  column < m_World.Zone.celss.columns ) {
                    if(false == HasCellObject(row, column)) {
                        m_Cells[row, column].SetObject(obj);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UnsetCellObject(int row, int column ) {
            if(m_World != null ) {
                if(row < m_World.Zone.celss.rows && column < m_World.Zone.celss.columns ) {
                    if(HasCellObject(row, column)) {
                        m_Cells[row, column].SetObject(null);
                        return true;
                    }
                }
            }
            return false;
        }

        public Hashtable GetInfo() {
            List<object> list = new List<object>();
            for(int i = 0; i < m_World.Zone.celss.rows; i++ ) {
                for(int j = 0; j < m_World.Zone.celss.columns; j++ ) {
                    list.Add(m_Cells[i, j].GetInfo());
                }
            }
            return new Hashtable {
                { (int)SPC.Cells, list.ToArray() }
            };
        }
    }
}
