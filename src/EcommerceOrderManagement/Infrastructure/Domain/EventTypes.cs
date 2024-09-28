namespace EcommerceOrderManagement.Infrastructure;

public enum EventTypes
{
    OrderCompletedWaitingProcessing = 0,
    OrderPaymentProcessing = 1,
    OrderPaymentDone = 2,
    PickingOrder = 3,
    // PaymentReceived = 1,
    // OrderShipped = 2,
}