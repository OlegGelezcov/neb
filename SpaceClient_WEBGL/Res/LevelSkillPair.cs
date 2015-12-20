using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif
using Common;

namespace Nebula.Client.Res {
    public class LevelSkillPair {
        public int level { get; private set; }
        public int skill { get; private set; }

#if UP
        public LevelSkillPair(UPXElement element) {
            level = element.GetInt("value");
            skill = Int32.Parse(element.GetString("skills"), System.Globalization.NumberStyles.HexNumber);
        }
#else
        public LevelSkillPair(XElement element ) {
            level = element.GetInt("value");
            skill = Int32.Parse(element.GetString("skills"), System.Globalization.NumberStyles.HexNumber);
        }
#endif
    }
}
