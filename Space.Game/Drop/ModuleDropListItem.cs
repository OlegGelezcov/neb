using Common;
using Space.Game;
using Space.Game.Drop;
using Space.Game.Inventory;
using Space.Game.Ship;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Nebula.Drop {
    public class ModuleDropListItem : DropListItem {

        public ShipModelSlotType moduleType { get; private set; }

        public ModuleDropListItem(InventoryObjectType category, string colorList, ShipModelSlotType type)
            : base( category, colorList ) {
            moduleType = type;
        }

        public ModuleDropListItem(XElement element) 
            : base(element) {
            moduleType = (ShipModelSlotType)Enum.Parse(typeof(ShipModelSlotType), element.GetString("type"));
        }

        public override ServerInventoryItem Roll(IRes resource, int level, Workshop workshop) {
            DropManager dropManager = DropManager.Get(resource);
            var cl = resource.colorLists.GetList(colorList);
            ObjectColor color = cl.Roll();
            string moduleTemplate = string.Empty;
            string setId = string.Empty;

            if(color != ObjectColor.green ) {
                moduleTemplate = resource.ModuleTemplates.RandomModule(workshop, moduleType).Id;
            } else {
                var generatedSetInfo = dropManager.GenerateSet(level, workshop, moduleType);
                if(!string.IsNullOrEmpty(generatedSetInfo.setId)) {
                    moduleTemplate = generatedSetInfo.moduleTemplateId;
                    setId = generatedSetInfo.setId;
                } else {
                    color = ObjectColor.orange;
                    moduleTemplate = resource.ModuleTemplates.RandomModule(workshop, moduleType).Id;
                }
            }

            ModuleDropper.ModuleDropParams dropParams = new ModuleDropper.ModuleDropParams(
                resource,
                moduleTemplate,
                level,
                Difficulty.none,
                new Dictionary<string, int>(),
                color,
                setId);
            ModuleDropper moduleDropper = dropManager.GetModuleDropper(dropParams);
            ShipModule module = moduleDropper.Drop() as ShipModule;
            return new ServerInventoryItem(module, 1);
        }
    }
}
