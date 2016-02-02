using Common;
using System;
using System.Xml.Linq;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class BotComponentData : ComponentData, IDatabaseComponentData {

        public BotItemSubType subType { get; private set; }
        public string botGroup { get; private set; }

        public BotComponentData(XElement e) {
            subType = (BotItemSubType)Enum.Parse(typeof(BotItemSubType), e.GetString("bot_sub_type"));

            if (e.HasAttribute("group")) {
                botGroup = e.GetString("group");
            } else {
                botGroup = string.Empty;
            }
        }

        public BotComponentData(BotItemSubType subType) {
            this.subType = subType;
            botGroup = string.Empty;
        }

        

        public BotComponentData(Hashtable hash) {
            subType = (BotItemSubType)(byte)hash.GetValue<int>((int)SPC.SubType, (int)BotItemSubType.None);
            botGroup = hash.GetValue<string>((int)SPC.Group, string.Empty);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Bot;
            }
        }



        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.SubType, (int)(byte)subType },
                { (int)SPC.Group, botGroup }
            };
        }
    }
}
