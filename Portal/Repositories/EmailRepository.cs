using VoteUp.Portal.Exceptions;
using VoteUp.Portal.Services;
using VoteUp.Portal.Services.Email;
using VoteUp.Portal.Services.Email.Util;
using VoteUp.PortalData;
using VoteUp.PortalData.Models.Interfaces;

namespace VoteUp.Portal.Repositories;

public interface IEmailRepository
{
	Task SendEmailAsync(EmailInput input);
	Task SendEmailAsync(string template, dynamic parameters, string receiver);
}

public class EmailRepository(
	VoteUpDbContext db,
	IEmailService emailService,
	IAuthContext authContext,
	ITemplatingService templatingService
) : IEmailRepository
{
	public async Task SendEmailAsync(EmailInput input)
	{
		var email = input.MapToEmail(authContext.UserId);

		var result = await emailService.SendAsync(email);

		db.Add(email);

		if (!result.IsSuccess)
		{
			await db.SaveChangesAsync();
			throw new ApiException("Greska prilikom slanja emaila");
		}

		email.Sent = DateTime.UtcNow;
		await db.SaveChangesAsync();
	}

	public async Task SendEmailAsync(string templateName, dynamic parameters, string receiver)
	{
		var templateData = await templatingService.GetDefaultEmailTemplate(templateName);

		EmailInput emailInput =
			new(
				Subject: templatingService.ParametrizeTemplate(
					templateData.SubjectTemplate,
					parameters
				),
				HtmlContent: templatingService.ParametrizeTemplate(
					templateData.TemplateHtmlContent,
					parameters
				),
				Receiver: receiver
			);


        await SendEmailAsync(emailInput);
	}
}
