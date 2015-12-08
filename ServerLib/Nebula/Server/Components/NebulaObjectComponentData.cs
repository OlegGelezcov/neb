using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class NebulaObjectComponentData : ComponentData, IDatabaseComponentData {
        public ItemType itemType { get; private set; }
        public Dictionary<byte, object> tags { get; private set; }
        public string badge { get; private set; }
        public float size { get; private set; } = 1f;
        public int subZoneID { get; private set; } = 0;
        

        public NebulaObjectComponentData(XElement element) {
            itemType = (ItemType)Enum.Parse(typeof(ItemType), element.GetString("item_type"));
            badge = element.GetString("badge");

            if(element.HasAttribute("size"))
                size = element.GetFloat("size");
            if(element.HasAttribute("subzone_id")) {
                subZoneID = element.GetInt("subzone_id");
            }
            tags = new Dictionary<byte, object>();
            var dump = element.Elements("tag").Select(te => {
                var kv = ParseTag(te);
                tags.Add(kv.Key, kv.Value);
                return kv.Key;
            }).ToList();

        }

        public NebulaObjectComponentData(ItemType itemType, Dictionary<byte, object> tags, string badge, float size, int subZone) {
            this.itemType = itemType;
            this.tags = tags;
            this.badge = badge;
            this.size = size;
            this.subZoneID = subZoneID;
        }

        public NebulaObjectComponentData(Hashtable hash) {
            itemType = (ItemType)(byte)hash.GetValue<int>((int)SPC.ItemType, (int)ItemType.Asteroid);
            tags = new Dictionary<byte, object>();
            badge = hash.GetValue<string>((int)SPC.Badge, string.Empty);
            size = hash.GetValue<float>((int)SPC.Size, 1f);
            subZoneID = hash.GetValue<int>((int)SPC.SubZoneID, 0);
        }
        

        public override ComponentID componentID {
            get {
                return ComponentID.NebulaObject;
            }
        }

        private KeyValuePair<byte, object> ParseTag(XElement element) {
            string value = element.GetString("value");
            TagType type = (TagType)Enum.Parse(typeof(TagType), element.GetString("type"));
            byte key = (byte)element.GetInt("key");
            object val = CommonUtils.ParseValue(value, type);
            return new KeyValuePair<byte, object>(key, val);
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.ItemType, (int)itemType },
                { (int)SPC.Badge, badge },
                { (int)SPC.Size, size },
                { (int)SPC.SubZoneID, subZoneID }
            };
        }
    }
}
