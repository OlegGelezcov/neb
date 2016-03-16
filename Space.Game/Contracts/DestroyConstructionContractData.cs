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

        public Race GenerateTargetRace(Race sourceRace) {
            switch(sourceRace) {
                case Race.Humans: {
                        if(Rand.Int() % 2 == 0 ) {
                            return Race.Borguzands;
                        } else {
                            return Race.Criptizoids;
                        }
                    }
                case Race.Borguzands: {
                        if(Rand.Int() % 2 == 0 ) {
                            return Race.Humans;
                        } else {
                            return Race.Criptizoids;
                        }
                    }
                case Race.Criptizoids: {
                        if(Rand.Int() % 2 == 0 ) {
                            return Race.Humans;
                        } else {
                            return Race.Borguzands;
                        }
                    }
                default: {
                        int val = Rand.Int() % 3;
                        if(val == 0 ) {
                            return Race.Humans;
                        } else if(val == 1 ) {
                            return Race.Borguzands;
                        } else {
                            return Race.Criptizoids;
                        }
                    }
            }
        }
    }
}
