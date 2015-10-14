namespace Nebula.Mmo.Items.Components {
    using Common;

    public class MmoPlanetComponent : MmoBaseComponent {
        public override ComponentID componentID {
            get {
                return ComponentID.Planet;
            }
        }

        public PlanetType planetType {
            get {
                if(item != null ) {
                    byte bPlanetType;
                    if(item.TryGetProperty<byte>((byte)PS.PlanetType, out bPlanetType)) {
                        return (PlanetType)bPlanetType;
                    }
                }
                return PlanetType.Planet;
            }
        }
    }
}
