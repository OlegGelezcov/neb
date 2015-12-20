namespace Nebula.Server.Login {
    public class GameRefId {

        private string m_GameRef;

        public GameRefId(string gameRefId ) {
            m_GameRef = gameRefId;
        }

        public string value {
            get {
                return m_GameRef;
            }
        }
    }
}
