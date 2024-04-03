namespace VoteUp.Portal.Services.Email.Util;

public static class DefaultEmailTemplates
{
	public const string TemplateExample = "template_example";
	public const string WelcomeWithVerification = "welcome_with_verification"; // Registration template - Not implemented

	//...

	public static Dictionary<string, string> Subjects = new() { { TemplateExample, "Dobrodošli {{user_first_name}} - Potvrdite vaš nalog" } };
}
