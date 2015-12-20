namespace Nebula.Server.Login {
    public class Email {
        private string m_Email;

        public Email(string email) {
            m_Email = email;
        }

        public string value {
            get {
                return m_Email;
            }
        }
    }
}
