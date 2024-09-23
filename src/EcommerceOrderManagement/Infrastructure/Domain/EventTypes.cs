namespace EcommerceOrderManagement.Domain.Infrastructure;

public enum EventTypes
{
    OrderCompletedWaitingProcessing = 0,
    OrderPaymentProcessing = 1,
    // PaymentReceived = 1,
    // OrderShipped = 2,
}