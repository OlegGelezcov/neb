using Nebula.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nebula.Game.Components {
    public class FounderCube : NebulaBehaviour, IDatabaseObject {

        public override int behaviourId {
            get {
                throw new NotImplementedException();
            }
        }

        public Hashtable GetDatabaseSave() {
            throw new NotImplementedException();
        }
    }
}
