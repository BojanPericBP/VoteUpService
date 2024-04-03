namespace VoteUp.Portal.Services.Email.Util;

public class EmailSendingError(string message, string? code = null) : Exception(message)
{
	public string? Code { get; set; } = code;

	public override string ToString() => $"{Code ?? ""} | {Message}";
}
