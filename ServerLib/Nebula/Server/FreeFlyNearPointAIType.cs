namespace Nebula.Server {

    [System.Serializable]
    public class FreeFlyNearPointAIType : CombatAIType {

        public float radius;

        public override MovingType movingType {
            get {
                return MovingType.FreeFlyNearPoint;
            }
        }
    }
}
