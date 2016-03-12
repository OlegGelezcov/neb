using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources.Zones {
    public class PlanetCell {
        public int row { get; private set; }
        public int column { get; private set; }
        public Vector3 position { get; private set; }

        public PlanetCell(int row, int column, Vector3 position) {
            this.row = row;
            this.column = column;
            this.position = position;
        }


        public static PlanetCell FromXml(XElement element) {
            int iRow = element.GetInt("row");
            int iColumn = element.GetInt("column");
            float[] arrPos = element.GetFloatArray("position");
            Vector3 pos = new Vector3(arrPos);
            return new PlanetCell(iRow, iColumn, pos);
        }
    }
}
