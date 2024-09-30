namespace EcommerceOrderManagement.SchedulerWorker.Configuration;

public class MailStripeSettings
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string FromEmail { get; set; }
    public string Subject { get; set; }
}