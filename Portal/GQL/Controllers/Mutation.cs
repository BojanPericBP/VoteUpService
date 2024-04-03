using System.Web;
using HotChocolate.Authorization;
using VoteUp.Portal.Repositories;
using VoteUp.Portal.Services.Email.Util;

namespace VoteUp.Portal.GQL.Controllers;

[Authorize]
public class Mutation
{
	public string TestMutation(string arg1, string? arg2)
	{
		return "Mutation called... kme";
	}

	public async Task<bool> SendTestEmail([Service(ServiceKind.Synchronized)] IEmailRepository emailRepository)
	{
		await emailRepository.SendEmailAsync(
			DefaultEmailTemplates.TemplateExample,
			new
			{
				user_first_name = "Nikola",
				platform_name = "VoteUp",
				verify_email_url_encoded = $"https://localhost:4200/{HttpUtility.UrlEncode("verify/nikola@invenit.io")}"
			},
			"nikola@invenit.io"
		);
		return true;
	}
}
