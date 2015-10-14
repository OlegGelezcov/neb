using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;

namespace Nebula.Client.Res {
    public class LevelSkillPair {
        public int level { get; private set; }
        public int skill { get; private set; }

        public LevelSkillPair(XElement element ) {
            level = element.GetInt("value");
            skill = Int32.Parse(element.GetString("skills"), System.Globalization.NumberStyles.HexNumber);
        }
    }
}
