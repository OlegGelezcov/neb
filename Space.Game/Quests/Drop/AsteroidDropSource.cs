using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Quests.Drop {
    public class AsteroidDropSource : DropSource {

        public string asteroidDataId { get; private set; }

        public AsteroidDropSource(string dataId ) : base(Common.ItemType.Asteroid) {
            this.asteroidDataId = dataId;
        }
    }
}
