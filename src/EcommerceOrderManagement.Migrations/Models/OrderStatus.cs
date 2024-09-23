namespace EcommerceOrderManagement.Migrations.Models;

public enum OrderStatus
{
    AwaitingProcessing = 0,      // AguardandoProcessamento
    ProcessingPayment = 1,       // ProcessandoPagamento
    PaymentCompleted = 2,        // PagamentoConcluido
    PickingOrder = 3,            // SeparandoPedido
    Completed = 4,               // Concluido
    WaitingForStock = 5,         // AguardandoEstoque
    Canceled = 6                 // Cancelado
}