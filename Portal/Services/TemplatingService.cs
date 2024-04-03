using System.Text;
using VoteUp.Portal.Exceptions;
using VoteUp.Portal.Services.Email.Util;

namespace VoteUp.Portal.Services;

public interface ITemplatingService
{
	string ParametrizeTemplate(string template, Dictionary<string, string> substitutions, int? repeatCount = 0);
	string ParametrizeTemplate(string template, dynamic substitutions, int? repeatCount = 0);

	Task<EmailTemplate> GetDefaultEmailTemplate(string defaultTemplateName);
	Task<string> LoadTemplate(string templateName);
}

public class TemplatingService(IWebHostEnvironment environment) : ITemplatingService
{
	public async Task<EmailTemplate> GetDefaultEmailTemplate(string defaultTemplateName)
	{
		return new EmailTemplate()
		{
			SubjectTemplate =
				DefaultEmailTemplates.Subjects.GetValueOrDefault(
					DefaultEmailTemplates.TemplateExample
				) ?? throw new ApiException("Template not found"),
			TemplateHtmlContent = await LoadTemplate(defaultTemplateName)
		};
	}

	public async Task<string> LoadTemplate(string templateName)
	{
		return await Task.FromResult(
			File.ReadAllText(
				System.IO.Path.Combine(
					environment.WebRootPath,
					"assets",
					"email_templates",
					$"{templateName}.html"
				)
			)
		);
	}

	public string ParametrizeTemplate(
		string template,
		Dictionary<string, string> substitutions,
		int? repeatCount = 0
	)
	{
		if (string.IsNullOrEmpty(template))
			return template;

		if (substitutions != null)
			for (int i = 0; i <= repeatCount; i++)
				foreach (KeyValuePair<string, string> entry in substitutions)
					template = template.Replace("{{" + entry.Key + "}}", entry.Value);

		return template;
	}

	public string ParametrizeTemplate(
		string template,
		dynamic substitutions,
		int? repeatCount = 0
	)
	{
		if (string.IsNullOrEmpty(template) || substitutions is null)
			return template;

		StringBuilder sb = new(template);

		for (int i = 0; i <= repeatCount; i++)
			foreach (var prop in substitutions.GetType().GetProperties())
				sb.Replace("{{" + prop.Name + "}}", prop.GetValue(substitutions, null) ?? "");

		return sb.ToString();
	}
}
