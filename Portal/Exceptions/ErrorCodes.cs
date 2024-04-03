namespace VoteUp.Portal.Exceptions;

public enum ErrorCode
{
    NotFound,

    // Auth
    InvalidCredentials,
    UserNotApproved,
    AccountLocked,
    LoginNotAllowed
}
