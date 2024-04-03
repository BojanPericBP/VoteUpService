using Microsoft.Extensions.Options;
using VoteUp.Portal.Services.Email.Providers.Sendgrid;
using VoteUp.PortalData.Models;

namespace PortalTest;

public class SendGridEmailServiceTests
{
    [Fact]
    public async void SendTestEmailWithSendGrid()
    {
        SendGridSettings settings = new()
        {
           ApiKey = "API KEY HERE",
           DefaultFrom = "senderMej@lHere",
           DefaultFromName = "VoteUpPortal",
           DefaultTo = "senderMej@lHere",
           Paused = false
        };

        IOptions<SendGridSettings> sendGridSettingsOptions = Options.Create(settings);

        SendGridEmailProvider sendGridEmailProvider = new(sendGridSettingsOptions);

        Email email = new(){
            Sender = "senderMej@lHere",
            Receiver = "nikola+emailtest@invenit.io",
            Subject = "Single email test",
            HtmlContent = "<h1>This is test message</h1><p>This is message in paragraph</p>"

        };
        var result = await sendGridEmailProvider.SendAsync(email);
        Assert.True(result.IsSuccess);
    }
}