using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using VoteUp.Portal.Services.Email.Util;

namespace VoteUp.Portal.Services.Email.Providers.Sendgrid;

public class SendGridEmailProvider : IEmailProvider
{
	private readonly string SENDGRID_CONTENT_MIME_TYPE = "text/html";
	private readonly SendGridSettings _settings;
	private readonly SendGridClient _client;
	private readonly ILogger<SendGridEmailProvider>? _logger;

	public SendGridEmailProvider(
		IOptions<SendGridSettings> options,
		ILogger<SendGridEmailProvider>? logger = null
	)
	{
		_settings = options.Value;
		_client = new SendGridClient(_settings.ApiKey);
		_logger = logger;
	}

	public async Task<EmailSendingResult> SendAsync(PortalData.Models.Email message)
	{
		if (_settings.Paused)
			return await Task.FromResult(new EmailSendingResult() { IsSuccess = true });

		if (string.IsNullOrEmpty(message.Sender) && _settings.DefaultFrom != null)
			message.Sender = _settings.DefaultFrom;

		if (string.IsNullOrEmpty(message.Receiver) && _settings.DefaultTo != null)
			message.Receiver = _settings.DefaultTo;

		SendGridMessage msg = MapEmailToSendGridMessage(message);
		Response resp = await _client.SendEmailAsync(msg);

		return await MapResponseToSendingResult(resp);
	}

	private SendGridMessage MapEmailToSendGridMessage(PortalData.Models.Email email)
	{
		SendGridMessage msg = new();

		msg.SetFrom(email.Sender, _settings.DefaultFromName);
		msg.AddTo(email.Receiver);
		msg.SetSubject(email.Subject);
		msg.AddContent(SENDGRID_CONTENT_MIME_TYPE, email.HtmlContent);

		return msg;
	}

	private async Task<EmailSendingResult> MapResponseToSendingResult(Response response)
	{
		EmailSendingResult result = new() { IsSuccess = response.IsSuccessStatusCode };

		if (!result.IsSuccess)
		{
			var sendGridResponseBody = await response.Body.ReadAsStringAsync();
			_logger?.LogError(sendGridResponseBody);
		}

		return result;
	}
}
