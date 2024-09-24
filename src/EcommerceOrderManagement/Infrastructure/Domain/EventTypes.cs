namespace EcommerceOrderManagement.Infrastructure;

public enum EventTypes
{
    OrderCompletedWaitingProcessing = 0,
    OrderPaymentProcessing = 1,
    OrderPaymentDone = 2,
    // PaymentReceived = 1,
    // OrderShipped = 2,
}