using Common;
using GameMath;
using Nebula.Game.Components;
using ServerClientCommon;
using Space.Game.Events;
using Space.Game.Inventory.Objects;
using Space.Game.Resources;
using Space.Game.Resources.Zones;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Space.Game.Objects {
    public class AsteroidContent {
        public int Count { get; set; }
        public MaterialObject Material { get; set; }

        public Hashtable GetInfo() {
            if (this.Material == null)
                return new Hashtable();
            Hashtable result = new Hashtable() { { (int)SPC.Count, this.Count }, { (int)SPC.Info, this.Material.GetInfo() } };
            return result;
        }
    }
}
