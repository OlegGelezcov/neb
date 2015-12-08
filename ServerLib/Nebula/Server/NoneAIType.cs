namespace Nebula.Server {
    [System.Serializable]
    public class NoneAIType : CombatAIType {

        public override MovingType movingType {
            get {
                return MovingType.None;
            }
        }

    }
}
