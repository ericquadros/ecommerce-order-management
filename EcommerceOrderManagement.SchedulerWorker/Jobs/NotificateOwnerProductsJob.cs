using System.Net;
using System.Net.Mail;
using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.SchedulerWorker.Configuration;
using Microsoft.Extensions.Configuration;
using Quartz;

namespace EcommerceOrderManagement.SchedulerWorker.Jobs;

public class NotificateOwnerProductsJob : IJob
{
    private readonly MailStripeSettings _mailStripeSettings;
    private readonly IOrderRepository _orderRepository;

    // Injeção de dependência do repositório de pedidos
    public NotificateOwnerProductsJob(
        IConfiguration configuration,
        IOrderRepository orderRepository)
    {
        _mailStripeSettings = configuration.GetSection("MailStripe").Get<MailStripeSettings>();
        _orderRepository = orderRepository;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        // Obter pedidos do dia anterior
        var orders = await GetOrdersFromPreviousDay();

        // Enviar a lista de pedidos por e-mail, um por um
        await SendEmailWithOrders(orders);
    }

    private async Task<List<Order>> GetOrdersFromPreviousDay()
    {
        var orders = await _orderRepository.GetOrdersFromPreviousDay();
        return orders;
    }

    private async Task SendEmailWithOrders(List<Order> orders)
    {
        using (var client = new SmtpClient(_mailStripeSettings.SmtpServer, _mailStripeSettings.Port)
               {
                   Credentials = new NetworkCredential(_mailStripeSettings.Username, _mailStripeSettings.Password),
                   EnableSsl = true
               })
        {
            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    var product = item.Product;

                    if (product == null || string.IsNullOrEmpty(product.Owner.OwnerEmail))
                    {
                        Console.WriteLine($"Produto sem e-mail do dono ou inválido para o item do pedido {order.Id}");
                        continue; // Ignora se o produto não tiver um dono válido
                    }

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_mailStripeSettings.FromEmail),
                        Subject = $"{_mailStripeSettings.Subject} - Pedido {order.Id}",
                        Body = $"O produto {product.Name} foi vendido no pedido {order.Id}. Quantidade: {item.Quantity}",
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(product.Owner.OwnerEmail); // Adiciona o e-mail do dono do produto

                    try
                    {
                        // Envia o e-mail
                        await client.SendMailAsync(mailMessage);
                        Console.WriteLine($"E-mail enviado com sucesso para {product.Owner.OwnerEmail} sobre o pedido {order.Id} e produto {product.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao enviar e-mail para {product.Owner.OwnerEmail}: {ex.Message}");
                    }
                }
            }
        }
    }
    
}