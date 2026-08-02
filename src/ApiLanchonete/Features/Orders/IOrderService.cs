namespace ApiLanchonete.Features.Orders;

public interface IOrderService
{
    Task<List<OrderDto>> GetOrders();
    Task<OrderDto> GetOrderById(Guid id);
    Task<OrderDto> CreateOrder(CreateOrderDto dto);
    Task UpdateOrder(Guid id, UpdateOrderDto dto);
    Task DeleteOrder(Guid id);
}