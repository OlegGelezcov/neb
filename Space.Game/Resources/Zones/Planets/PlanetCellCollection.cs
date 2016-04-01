using Common;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources.Zones.Planets {
    public class PlanetCellCollection {
        private List<PlanetCellData> m_Cells;
        private int m_Rows;
        private int m_Columns;


        public PlanetCellCollection(XElement cellsElement) {
            m_Cells = new List<PlanetCellData>();
            if(cellsElement != null ) {
                m_Cells = cellsElement.Elements("cell").Select(ce => {
                    return new PlanetCellData(ce);
                }).ToList();

                if(cellsElement.Attribute("rows") != null ) {
                    m_Rows = cellsElement.GetInt("rows");
                }
                if(cellsElement.Attribute("columns") != null ) {
                    m_Columns = cellsElement.GetInt("columns");
                }
            }
        }

        public PlanetCellData GetCell(int row, int column) {
            foreach(var cell in m_Cells) {
                if(cell.row == row && cell.column == column ) {
                    return cell;
                }
            }
            return null;
        }

        public List<PlanetCellData> cells {
            get {
                return m_Cells;
            }
        }

        public int rows {
            get {
                return m_Rows;
            }
        }

        public int columns {
            get {
                return m_Columns;
            }
        }
    }
}
