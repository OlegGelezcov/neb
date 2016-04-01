using Nebula.Engine;
using Space.Game;

namespace Nebula {
    public class PlanetWorldCellBoard {

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

        private bool HasCellObject(int row, int column ) {
            if(row < m_World.Zone.celss.rows && column < m_World.Zone.celss.columns ) {
                return m_Cells[row, column].hasCellObject;
            }
            return false;
        }

        public bool SetCellObject(int row, int column, NebulaObject obj ) {
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
    }
}
