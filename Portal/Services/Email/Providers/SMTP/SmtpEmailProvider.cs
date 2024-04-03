using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using VoteUp.Portal.Services.Email.Util;

namespace VoteUp.Portal.Services.Email.Providers.SMTP;

public class SmtpEmailProvider : IEmailProvider
{
	private readonly SmtpSettings _settings;
	private readonly ILogger<SmtpEmailProvider> _logger;
	private readonly SmtpClient _client;

	public SmtpEmailProvider(IOptions<SmtpSettings> options, ILogger<SmtpEmailProvider> logger)
	{
		_logger = logger;
		_settings = options.Value;
		_client = new SmtpClient()
		{
			Host = _settings.Host,
			Port = _settings.Port,
			UseDefaultCredentials = false,
			Credentials = new NetworkCredential(_settings.Username, _settings.Password),
			EnableSsl = _settings.EnableSsl
		};
	}

	public async Task<EmailSendingResult> SendAsync(PortalData.Models.Email message)
	{
		if (_settings.Paused)
			return await Task.FromResult(new EmailSendingResult() { IsSuccess = true });

		if (string.IsNullOrEmpty(message.Sender) && _settings.DefaultFrom != null)
			message.Sender = _settings.DefaultFrom;

		if (string.IsNullOrEmpty(message.Receiver) && _settings.DefaultTo != null)
			message.Receiver = _settings.DefaultTo;

		MailMessage mailMessage = MapEmailToMailMessage(message);

		try
		{
			_client.SendAsync(mailMessage, message.Sender);
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return new EmailSendingResult() { IsSuccess = false };
		}

		return new EmailSendingResult() { IsSuccess = true };
	}

	private static MailMessage MapEmailToMailMessage(PortalData.Models.Email email)
	{
		MailMessage msg =
			new()
			{
				Subject = email.Subject,
				IsBodyHtml = true,
				From = new MailAddress(email.Sender),
				Body = email.HtmlContent
			};
		msg.To.Add(email.Receiver);

		return msg;
	}

	~SmtpEmailProvider()
	{
		_client.Dispose();
	}
}
