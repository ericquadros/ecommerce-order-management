using EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Repositories;
using EcommerceOrderManagement.Infrastructure;
using EcommerceOrderManagement.Infrastructure.EFContext;
using EcommerceOrderManagement.Infrastructure.Http;
using EcommerceOrderManagement.Infrastructure.Interfaces;
using EcommerceOrderManagement.OrderManagementContext.Orders.Domain.Entities;
using EcommerceOrderManagement.PaymentManagementContext.Payments.Application.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceOrderManagement.Domain.PaymentManagementContext.Payments.Application.EventHandlers;

public class ProcessProcessingPaymentEventHandler
{
    private readonly OrderRepository _orderRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProcessProcessingPaymentEventHandler> _logger;
    private readonly IMessageBroker _messageBroker;
    private string checkoutPaymentsHttp;

    public ProcessProcessingPaymentEventHandler(
        OrderRepository orderRepository,
        IConfiguration configuration,
        ILogger<ProcessProcessingPaymentEventHandler> logger,
        IMessageBroker messageBroker)
    {
        _orderRepository = orderRepository;
        _configuration = configuration;
        _logger = logger;
        _messageBroker = messageBroker;

        checkoutPaymentsHttp = _configuration.GetValue<string>("IntegrationsHttp:CheckoutPaymentsHttp");
        if (string.IsNullOrWhiteSpace(checkoutPaymentsHttp))
            throw new ArgumentNullException("IntegrationsHttp:CheckoutPaymentsHttp cannot be null, set in appsettings");
    }

    public async Task<Result<Order>> HandleAsync(OrderProcessingPaymentStatusChangedEvent orderEvent)
    {
        if (orderEvent?.Object?.Id is null)
            return Result.Failure("The order is not setted in the event.");

        var order = await _orderRepository.GetOrderCompleteAsync(orderEvent?.Object?.Id);
        
        Result<Order> result;

        if (order.PixPayment is not null)
            result = await ProcessPixPayment(order, checkoutPaymentsHttp);
        else if (order.CardPayment is not null)
            result = await ProcessCardPayment(order, checkoutPaymentsHttp);
        else
            return Result.Failure("No valid payment method found.");

        if (result.IsSuccess)
        {
            order.PaymentCompleted();

            await _orderRepository.UpdateOrderAsync(order);
            
            // Publishing domain events
            foreach (var domainEvent in order.Events)
                await _messageBroker.ProduceMessageAsync(domainEvent);

            return order;
        }

        order.CancelOrder();
        _logger.LogError("Order was cancelled because it ocurred error on payment");
        
        await _orderRepository.UpdateOrderAsync(order);

        return order;
    }
    
    private async Task<Result<Order>> ProcessPixPayment(Order order, string checkoutPaymentsHttp)
    {
        var httpHelper = new HttpHelper(checkoutPaymentsHttp);
        var response = await httpHelper.ExecuteAsync<ApiResponse<SucessResponsePix>>(HttpRequestMethod.POST, "/pay/pix", new
        {
            pedidoId = order.Id,
            valor = order.TotalAmount,
            metodoPagamento = "PIX"
        });

        return response.status.Equals("success") ? order : Result.Failure($"Error: {response.Body.message}");
    }

    private async Task<Result<Order>> ProcessCardPayment(Order order, string checkoutPaymentsHttp)
    {
        var httpHelper = new HttpHelper(checkoutPaymentsHttp);
        var response = await httpHelper.ExecuteAsync<ApiResponse<SuccessResponse>>(HttpRequestMethod.POST, "/pay/credit-card", new
        {
            pedidoId = order.Id,
            valor = order.TotalAmount,
            parcelas = order.CardPayment.Installments,
            metodoPagamento = "CARTAO",
            operacao = "CREDITO" 
        });

        return response.status.Equals("success") ? order : Result.Failure($"Error: {response.Body.Message}"); //HandlePaymentResponse(order, response);
    }
    
    public record SucessResponsePix(string transactionId, string message);
    public record ApiResponse<T>(string status, T Body);
    public record SuccessResponse(string Status, string TransactionId, string Message);
    public record ErrorResponse(string Status, string Message);
}