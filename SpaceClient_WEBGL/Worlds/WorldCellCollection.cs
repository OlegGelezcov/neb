using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using ServerClientCommon;

namespace Nebula.Client.Worlds {
    public class WorldCellCollection : IInfoParser {

        public List<WorldCell> cells { get; private set; }

        public void ParseInfo(Hashtable info) {
            if(cells == null ) {
                cells = new List<WorldCell>();
            } else {
                cells.Clear();
            }

            if(info.ContainsKey((int)SPC.Cells)) {
                object[] cellArr = info[(int)SPC.Cells] as object[];
                if(cellArr != null ) {
                    foreach(object cObj in cellArr ) {
                        Hashtable cellHash = cObj as Hashtable;
                        if(cellHash != null ) {
                            cells.Add(new WorldCell(cellHash));
                        }
                    }
                }
            }
        }

        public WorldCell GetCell(int row, int column ) {
            if(cells != null ) {
                foreach(var c in cells ) {
                    if(c.row == row && c.column == column ) {
                        return c;
                    }
                }
            }
            return null;
        }

        public WorldCellCollection() {
            cells = new List<WorldCell>();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            if(cells != null ) {
                foreach(var cell in cells ) {
                    sb.AppendLine(cell.ToString());
                }
            }
            return sb.ToString();
        }
    }
}
