using Common;
using System;
using System.Xml.Linq;
using ExitGames.Client.Photon;
using ServerClientCommon;

namespace Nebula.Server.Components {
    public class BotComponentData : ComponentData, IDatabaseComponentData {

        public BotItemSubType subType { get; private set; }

        public BotComponentData(XElement e) {
            subType = (BotItemSubType)Enum.Parse(typeof(BotItemSubType), e.GetString("bot_sub_type"));
        }

        public BotComponentData(BotItemSubType subType) {
            this.subType = subType;
        }

        public BotComponentData(Hashtable hash) {
            subType = (BotItemSubType)(byte)hash.GetValue<int>((int)SPC.SubType, (int)BotItemSubType.None);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Bot;
            }
        }



        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.SubType, (int)(byte)subType }
            };
        }
    }
}
