using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Inventory.DropList {
    public class PetSchemeDropItem : DropItem {

        private PetColor m_PetColor;
        private string m_Template;


        public PetSchemeDropItem(string template, PetColor petColor, int min, int max, float prob) 
            : base(min, max, prob, InventoryObjectType.pet_scheme) {
            m_PetColor = petColor;
            m_Template = template;
        }

        public PetColor petColor {
            get {
                return m_PetColor;
            }
        }

        public string template {
            get {
                return m_Template;
            }
        }
    }
}
