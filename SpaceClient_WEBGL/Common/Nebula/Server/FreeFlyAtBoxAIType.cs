using GameMath;

namespace Nebula.Server {
    [System.Serializable]
    public class FreeFlyAtBoxAIType : CombatAIType {

        public MinMax corners;

        public override MovingType movingType {
            get {
                return MovingType.FreeFlyAtBox;
            }
        }
    }
}
