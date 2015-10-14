namespace Nebula.Mmo.Items.Components {
    using UnityEngine;
    using System.Collections;
    using Common;
    using System;

    public class MmoAsteroidComponent : MmoBaseComponent {

        public object[] asteroidContent {
            get {
                if (item != null) {
                    object[] content = null;
                    if (item.TryGetProperty<object[]>((byte)PS.AsteroidContent, out content)) {
                        return content;
                    }
                }
                return new object[] { };
            }
        }

        public string asteroidName {
            get {
                if (item != null) {
                    string name = "";
                    if (item.TryGetProperty<string>((byte)PS.Name, out name)) {
                        return name;
                    }
                }
                return string.Empty;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Asteroid;
            }
        }

        public string dataID {
            get {
                if (item == null) {
                    return string.Empty;
                }
                string dataid = string.Empty;
                if (item.TryGetProperty<string>((byte)PS.DataId, out dataid)) {
                    return dataid;
                }
                return string.Empty;
            }
        }
    }
}