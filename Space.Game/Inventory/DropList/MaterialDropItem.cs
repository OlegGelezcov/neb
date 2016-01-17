using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.DropList {
    public class MaterialDropItem : DropItem {
        private string m_TemplateId;

        public MaterialDropItem(string templateId, int min, int max, float prob)
            : base(min, max, prob, Common.InventoryObjectType.Material) {
            m_TemplateId = templateId;
        }

        public string templateId {
            get {
                return m_TemplateId;
            }
        }
    }
}
