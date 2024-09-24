using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Infrastructure.Mail;

public class CustomerEmailService
{
    private readonly ILogger<CustomerEmailService> _logger;

    public CustomerEmailService(IConfiguration configuration, ILogger<CustomerEmailService> logger)
    {
        _logger = logger;
    }

    public void SendNotificationEmail(string subject, string body, string email)
    {
        string fromEmail = string.Empty;

        try
        {
            _logger.LogInformation($"E-mail: {email}, E-mail subject:{subject} , Body:{body} sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when to try send e-mail to sales team: {ex.Message}");
        }
    }
    
    public async Task SendOrderStatusChangeEmail(string toEmail, string orderId, string newStatus)
    {
        string fromEmail = string.Empty; // _configuration["Email:From"]; // Configuração do e-mail remetente
        string subject = $"Atualização no status do pedido #{orderId}";
        string body = $"O status do seu pedido foi atualizado para: {newStatus}";

        try
        {
            _logger.LogInformation($"E-mail sent successfully to the customer about order {orderId} status: {newStatus}.");
            _logger.LogInformation($"E-mail subject:{subject} body:{body} sent successfully.");
            // SendNotificationEmail(subject, body, toEmail); // Chama o método de envio de e-mail
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error when to try send e-mail to the customer: {ex.Message}");
        }
    }
}