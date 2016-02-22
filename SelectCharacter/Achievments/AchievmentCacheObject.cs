using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Achievments {
    public class AchievmentCacheObject {
        public string characterId { get; set; }
        public float updateFromDBTime { get; set; }
        public AchievmentDocument document { get; set; }
    }
}
