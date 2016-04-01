using Common;
using GameMath;
using Space.Game;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class DestroyConstructionContractData : ContractData {

        public DestroyConstructionContractData(XElement element)
            : base(element) { }

        public BotItemSubType GenerateConstructionType() {
            List<BotItemSubType> types = new List<BotItemSubType> { BotItemSubType.Outpost, BotItemSubType.MainOutpost, BotItemSubType.Turret };
            return types.AnyElement();
        }


    }
}
