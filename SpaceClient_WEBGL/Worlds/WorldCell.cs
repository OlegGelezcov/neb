using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Worlds {
    public class WorldCell : IInfoParser {
        public int row { get; private set; }
        public int column { get; private set; }
        public float[] position { get; private set; }
        public bool hasCellObject { get; private set; }
        public string cellObjectId { get; private set; } = string.Empty;
        public byte cellObjectType { get; private set; }

        public WorldCell(Hashtable hash) {
            ParseInfo(hash);
        }

        public void ParseInfo(Hashtable info) {
            row = info.GetValueInt((int)SPC.Row);
            column = info.GetValueInt((int)SPC.Column);
            position = info.GetValueFloatArray((int)SPC.Position);
            hasCellObject = info.GetValueBool((int)SPC.HasCellObject);
            if(hasCellObject ) {
                cellObjectId = info.GetValueString((int)SPC.ItemId);
                cellObjectType = info.GetValueByte((int)SPC.ItemType);
            } else {
                cellObjectId = string.Empty;
                cellObjectType = 0;
            }
        }

        public override string ToString() {
            string str = string.Format("cell [{0},{1}], pos: {2}, has object? {3}", row, column, position.StringArray());
            if(hasCellObject) {
                str += string.Format("\ncell object {0}:{1}", (ItemType)cellObjectType, cellObjectId);
            }
            return str;
        }
    }

}
