using GameMath;

namespace Nebula.Server {
    [System.Serializable]
    public class PatrolAIType : CombatAIType {

        public Vector3 firstPoint;

        public Vector3 secondPoint;

        public override MovingType movingType {
            get {
                return MovingType.Patrol;
            }
        }
    }
}
