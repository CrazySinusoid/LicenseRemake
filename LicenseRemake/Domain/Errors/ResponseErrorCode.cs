namespace LicenseRemake.Domain.Errors
{
    public enum ResponseErrorCode
    {
        NoError = 0,

        EntityNotFound = 302,
        EntityAlreadyExists = 304,

        DatabaseError = 301,

        UserNotFound = 522,
        UserIsBlocked = 523,

        InvalidCredentials = 903,

        AccessForbidden = 901,

        UndefinedError = 991
    }
}
