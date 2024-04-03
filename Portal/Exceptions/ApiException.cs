namespace VoteUp.Portal.Exceptions;

public partial class ApiException : Exception
{
	public string? Code { get; set; }

	public ApiException(string message)
		: base(message) { }

	public ApiException(string message, string code)
		: base(message)
	{
		Code = code;
	}
}
