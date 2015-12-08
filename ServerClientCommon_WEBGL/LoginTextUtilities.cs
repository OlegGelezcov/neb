using System.Text.RegularExpressions;

namespace ServerClientCommon {
    public class LoginTextUtilities {

        private const int MIN_LOGIN_PASSW_LENGTH = 6;

        public bool IsLoginLengthValid(string login) {
            return (login.Length >= MIN_LOGIN_PASSW_LENGTH);
        }

        public bool IsPasswordLengthValid(string password) {
            return (password.Length >= MIN_LOGIN_PASSW_LENGTH);
        }

        public bool IsLoginCharactersValid(string login) {
            return Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$");
        }

        public bool IsPasswordCharactersValid(string password) {
            return Regex.IsMatch(password, @"^[a-zA-Z0-9_]+$");
        }
    }
}
