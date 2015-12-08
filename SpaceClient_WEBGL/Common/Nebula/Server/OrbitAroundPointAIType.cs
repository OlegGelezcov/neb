namespace Nebula.Server {

    [System.Serializable]
    public class OrbitAroundPointAIType : CombatAIType {
        public float phiSpeed;
        public float thetaSpeed;
        public float radius;

        public override MovingType movingType {
            get {
                return MovingType.OrbitAroundPoint;
            }
        }
    }
}
