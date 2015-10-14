using Common;

namespace Nebula.Mmo.Items.Components {
    public class MmoMiningStationComponent : MmoBaseComponent {
        public override ComponentID componentID {
            get {
                return ComponentID.MiningStation;
            }
        }

        /// <summary>
        /// Find owned player ID in item properties
        /// </summary>
        public string ownedPlayerID {
            get {
                if(item != null ) {
                    string id = string.Empty;
                    if(item.TryGetProperty<string>((byte)PS.DataId, out id)) {
                        return id;
                    }
                }
                return string.Empty;
            }
        }
    }
}
