﻿using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Inventory.DropList {
    public class DropItemFactory {

        public DropItem Create(XElement element) {
            InventoryObjectType type = (InventoryObjectType)Enum.Parse(typeof(InventoryObjectType), element.GetString("type"));
            int min = element.GetInt("min");
            int max = element.GetInt("max");
            float prob = element.GetFloat("prob");

            switch(type) {
                case InventoryObjectType.Weapon:
                    return new WeaponDropItem(min, max, prob);
                case InventoryObjectType.Scheme:
                    return new SchemeDropItem(min, max, prob);
                case InventoryObjectType.Material: {
                        string template = element.GetString("template");
                        return new MaterialDropItem(template, min, max, prob);
                    }
                case InventoryObjectType.nebula_element: {
                        string template = element.GetString("template");
                        return new NebulaElementDropItem(template, min, max, prob);
                    }
                case InventoryObjectType.craft_resource: {
                        string template = element.GetString("template");
                        return new CraftResourceDropItem(template, min, max, prob);
                    }
                case InventoryObjectType.pet_scheme: {
                        PetColor color = (PetColor)Enum.Parse(typeof(PetColor), element.GetString("color"));
                        string template = element.GetString("template");
                        return new PetSchemeDropItem(template, color, min, max, prob);
                    }
                case InventoryObjectType.contract_item: {
                        string template = element.GetString("template");
                        string contract = element.GetString("contract");
                        return new ContractObjectDropItem(template, contract, min, max, prob);
                    }
                case InventoryObjectType.planet_resource_hangar: {
                        return new PlanetHangarDropItem(min, max, prob);
                    }
                case InventoryObjectType.planet_resource_accelerator: {
                        return new PlanetResourceAcceleratorDropItem(min, max, prob);
                    }
                default:
                    return null;
            }
        }
    }
}
