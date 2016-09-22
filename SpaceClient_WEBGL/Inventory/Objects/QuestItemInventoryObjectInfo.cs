using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Inventory.Objects {
    public class QuestItemInventoryObjectInfo : IInventoryObjectInfo {

        public QuestItemInventoryObjectInfo(Hashtable hash) {
            ParseInfo(hash);
        }

        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public bool isNew {
            get;
            private set;
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get;
            private set;
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.quest_item;
            }
        }

        public string questId {
            get;
            private set;
        }

        public bool interactable {
            get;
            private set;
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

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            Id = info.GetValueString((int)SPC.Id);
            questId = info.GetValueString((int)SPC.Quest);
            isNew = info.GetValueBool((int)SPC.IsNew);
            interactable = info.GetValueBool((int)SPC.Interactable);
        }
    }
}
