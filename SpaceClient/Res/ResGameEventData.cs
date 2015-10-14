using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Res {
    public class ResGameEventData {
        public string id { get; set; }
        public float cooldown { get; set; }
        public float radius { get; set; }
        public string descriptionId { get; set; }
        public float[] position { get; set; }
    }
}
