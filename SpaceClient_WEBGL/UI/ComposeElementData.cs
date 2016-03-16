using System;
using System.Collections.Generic;

namespace Nebula.Client.UI {
    public abstract class ComposeElementData {
        private int[] m_Color;

        public ComposeElementData(UniXMLElement element) {
            if(element.HasAttribute("color")) {
                string colorString = element.GetString("color");
                string[] tokens = colorString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> colorList = new List<int>();
                foreach(string tok in tokens ) {
                    colorList.Add(int.Parse(tok));
                }
                while(colorList.Count < 4) {
                    colorList.Add(255);
                }
                m_Color = colorList.ToArray();
            } else {
                m_Color = new int[] { 255, 255, 255, 255 };
            }
        }

        public int[] color {
            get {
                return m_Color;
            }
        }

        public abstract ComposeElementType type { get; }
    }
}
