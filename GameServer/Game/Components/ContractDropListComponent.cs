using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nebula.Server.Components;
using Space.Game;
using Nebula.Engine;
using Nebula.Game.Contracts;
using Nebula.Inventory.DropList;
using ExitGames.Logging;

namespace Nebula.Game.Components {
    public class ContractDropListComponent : DropListComponent {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public override void Init(DropListComponentData data) {
            base.Init(data);
        }

        public override ActorDropListPair GetDropList(DamageInfo actor) {

            if(actor.DamagerType == Common.ItemType.Avatar ) {

                NebulaObject playerObject;

                if(nebulaObject.mmoWorld().TryGetObject((byte)actor.DamagerType, actor.DamagerId, out playerObject)) {

                    var contractManager = playerObject.GetComponent<ContractManager>();

                    if(contractManager) {

                        var contractDropList = contractDropItems;

                        if(contractDropList.Count > 0 ) {
                            var outputList = contractManager.FilterFoundItemContractForItems(
                                contractDropList.Select(s => (s as ContractObjectDropItem).templateId).ToList()
                                );

                            if(outputList.Count > 0 ) {

                                List<DropItem> newItems = new List<DropItem>();

                                foreach(string itId in outputList) {
                                    var dropItem = dropList.GetContractObjectItem(itId);
                                    if(dropItem != null ) {
                                        s_Log.InfoFormat("select drop item: {0}", dropItem.templateId);
                                        newItems.Add(dropItem);
                                    }
                                }

                                return new ActorDropListPair(actor, new ItemDropList(newItems));
                            }
                        }
                    }
                }
            }

            return new ActorDropListPair(actor, new ItemDropList());
        }

        private List<DropItem> contractDropItems {
            get {
                List<DropItem> ciList = new List<DropItem>();
                foreach(var di in dropList.items) {
                    if(di.type == Common.InventoryObjectType.contract_item ) {
                        ciList.Add(di);
                    }
                }
                return ciList;
            }
        }
    }
}
