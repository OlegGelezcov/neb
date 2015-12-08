using GameMath;

namespace Nebula.Server {

    [System.Serializable]
    public class FollowPathAIType : CombatAIType  {
        public Vector3[] path;


        public override MovingType movingType {
            get {
                return MovingType.FollowPathCombat;
            }
        }
    }
}
