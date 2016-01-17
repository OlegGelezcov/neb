using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.DropList {
    public class CraftResourceDropItem : DropItem {

        private string m_TemplateId;

        public CraftResourceDropItem(string templateId, int min, int max, float prob)
            : base(min, max, prob, Common.InventoryObjectType.craft_resource) {
            m_TemplateId = templateId;
        }

        public string templateId {
            get {
                return m_TemplateId;
            }
        }
    }
}
