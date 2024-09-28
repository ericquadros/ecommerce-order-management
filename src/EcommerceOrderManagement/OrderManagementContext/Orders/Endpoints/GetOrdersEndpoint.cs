using EcommerceOrderManagement.OrderManagementContext.Orders.Application.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CreateOrderCommand = EcommerceOrderManagement.Domain.OrderManagementContext.Orders.Application.Commands.CreateOrderCommand;

namespace EcommerceOrderManagement.OrderManagementContext.Endpoints;

using FastEndpoints;

public class GetOrdersEndpoint : Endpoint<EmptyRequest>
{
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersEndpoint(GetOrdersQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/orders");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get a list of orders";
            s.Description = "Retrieve a paginated list of orders.";
            s.ExampleRequest = new GetOrdersRequest(1, 20);
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken cancellationToken)
    {
        var defaultFirstPage = 1;
        var defaultPageSize = 20;
        var page = Query<int>("page", false); 
        var pageSize = Query<int>("pageSize", false);

        page = page != 0  ? page : defaultFirstPage;
        pageSize = pageSize != 0  ? pageSize : defaultPageSize;

        var result = await _handler.Handle(page, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            await SendAsync(new { Error = result.Error }, 400, cancellationToken);
            return;
        }

        await SendOkAsync(new { Data = result.Value }, cancellationToken);
    }
}

public record GetOrdersRequest([FromQuery] int? Page,  [FromQuery] int? PageSize);
