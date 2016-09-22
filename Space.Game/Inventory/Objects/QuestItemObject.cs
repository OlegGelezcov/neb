using Space.Game.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Inventory.Objects {
    public class QuestItemObject : IInventoryObject {

        private string m_QuestId;

        public QuestItemObject(string id, string questId ) {
            this.Id = id;
            this.m_QuestId = questId;
            this.interactable = DefineInteractable(id);
        }

        public QuestItemObject(Hashtable hash) {
            ParseInfo(hash);
        }

        public string questId {
            get {
                return m_QuestId;
            }
        }
        public bool binded {
            get {
                return true;
            }
        }

        public string Id {
            get;
            private set;
        }

        public bool isNew {
            get;
            private set;
        }

        public int Level {
            get {
                return 1;
            }
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

        public bool splittable {
            get {
                return false;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.quest_item;
            }
        }

        public bool interactable {
            get; private set;
        }

        public void Bind() {
        }

        public Hashtable GetInfo() {
            rawHash = InventoryUtils.ItemHash(Id, Level, ObjectColor.white, Type, (PlacingType)placingType, binded, splittable);
            rawHash.Add((int)SPC.Quest, questId);
            rawHash.Add((int)SPC.IsNew, isNew);
            rawHash.Add((int)SPC.Interactable, interactable);
            return rawHash;
        }

        public void ParseInfo(Hashtable info) {
            rawHash = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_QuestId = info.GetValue<string>((int)SPC.Quest, string.Empty);
            isNew = info.GetValue<bool>((int)SPC.IsNew, false);
            interactable = info.GetValue<bool>((int)SPC.Interactable, true);
        }

        public void ResetNew() {
            isNew = false;
        }

        public void SetNew(bool val) {
            isNew = val;
        }

        private bool DefineInteractable(string id) {
            switch(id) {
                case "latir001":
                    return false;
                case "analyzer001":
                    return true;
            }
            return false;
        }
    }
}
