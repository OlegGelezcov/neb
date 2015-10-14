using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class RespawnData {
        public string ID { get; set; } = string.Empty;
        public byte Type { get; set; }
        public float time { get; set; } = 0f;
        public bool handled { get; set; } = false;
    }
}
