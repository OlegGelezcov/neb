using Common;
using Nebula.Engine;
using System.Collections;

namespace Nebula.Game.Components.PlanetObjects {
    public abstract class PlanetObjectBase : NebulaBehaviour, IDatabaseObject {


        public override int behaviourId {
            get {
                return (int)ComponentID.PlanetBasedObject;
            }
        }

        public abstract Hashtable GetDatabaseSave();

    }
}
