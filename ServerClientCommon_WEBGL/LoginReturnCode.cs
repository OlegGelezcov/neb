namespace ServerClientCommon {
    public enum LoginReturnCode : int {
        Ok = 0,
        UserWithSameLoginAlreadyExists = 1,
        //InvaligLoginCharacter = 2,
        //InvaligLoginLength = 3,
        //UpdateGameRefOnClient = 4,
        UserWithLoginAndPasswordNotFound = 5,
        UserLoginOrPasswordIncorrect = 6,
        LoginVeryShort = 7,
        PasswordVeryShort = 8,
        LoginHasInvalidCharacters = 9,
        PasswordHasInvalidCharacters = 10,
        EmailInvalid = 11,
        UserNotFound = 12,
        UserWithSuchEmailAlreadyExists = 13,
        UnknownError = 14,
        NoPasses = 15
    }
}
