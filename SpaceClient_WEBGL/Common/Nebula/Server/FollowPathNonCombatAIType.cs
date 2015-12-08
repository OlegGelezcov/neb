using GameMath;

namespace Nebula.Server {
    [System.Serializable]
    public class FollowPathNonCombatAIType  : AIType {

        public Vector3[] path;


        public override MovingType movingType {
            get {
                return MovingType.FollowPathNonCombat;
            }
        }

    }
}
