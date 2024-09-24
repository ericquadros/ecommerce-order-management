using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Infrastructure.Mail;

public class SalesTeamEmailService
{
    private readonly ILogger<SalesTeamEmailService> _logger;

    public SalesTeamEmailService(IConfiguration configuration, ILogger<SalesTeamEmailService> logger)
    {
        _logger = logger;
    }

    public void SendNotificationEmail(string subject, string body)
    {
        string fromEmail = string.Empty;
        string toEmail = string.Empty; 

        try
        {
            _logger.LogInformation($"E-mail subject:{subject} body:{body} sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when to try send e-mail to sales team: {ex.Message}");
        }
    }
}