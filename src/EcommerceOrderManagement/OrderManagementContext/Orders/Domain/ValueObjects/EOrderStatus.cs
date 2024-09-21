namespace EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Domain.ValueObjects;

public enum EOrderStatus
{
    AwaitingProcessing = 0,
    ProcessingPayment = 1, 
    PaymentCompleted = 2,  
    PickingOrder = 3,      
    Completed = 4,         
    WaitingForStock = 5,   
    Canceled = 6           
}