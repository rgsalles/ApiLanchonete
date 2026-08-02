using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Orders;

public class OrderService(AppDbContext context) : IOrderService
{
    public async Task<List<OrderDto>> GetOrders()
    {
        return await context.Orders
            .AsNoTracking()
            .Select(o => new OrderDto
            {
                Id = o.Id,
                ClientId = o.ClientId,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                CreatedBy = o.CreatedBy,
                UpdatedAt = o.UpdatedAt,
                UpdatedBy = o.UpdatedBy,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OrderDto> GetOrderById(Guid id)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            throw new NotFoundException($"Order with ID {id} not found.");

        return new OrderDto
        {
            Id = order.Id,
            ClientId = order.ClientId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            CreatedBy = order.CreatedBy,
            UpdatedAt = order.UpdatedAt,
            UpdatedBy = order.UpdatedBy,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }

    public async Task<OrderDto> CreateOrder(CreateOrderDto dto)
    {
        var client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.ClientId);

        if (client is null)
            throw new NotFoundException($"Client with ID {dto.ClientId} not found.");

        var productIds = dto.Items
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        var products = await context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        if (products.Count != productIds.Count)
            throw new NotFoundException("One or more products were not found.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            ClientId = dto.ClientId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            Items = []
        };

        decimal totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var product = products[item.ProductId];

            if (!product.Active)
                throw new BadRequestException($"Product '{product.Name}' is inactive.");

            var totalPrice = product.Price * item.Quantity;

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                TotalPrice = totalPrice
            });

            totalAmount += totalPrice;
        }

        order.TotalAmount = totalAmount;

        context.Orders.Add(order);

        await context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            ClientId = order.ClientId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            CreatedBy = order.CreatedBy,
            UpdatedAt = order.UpdatedAt,
            UpdatedBy = order.UpdatedBy,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = products[i.ProductId].Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }

    public async Task UpdateOrder(Guid id, UpdateOrderDto dto)
    {
        var order = await context.Orders.FindAsync(id);

        if (order is null)
            throw new NotFoundException($"Order with ID {id} not found.");

        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteOrder(Guid id)
    {
        var order = await context.Orders.FindAsync(id);

        if (order is null)
            throw new NotFoundException($"Order with ID {id} not found.");

        context.Orders.Remove(order);

        await context.SaveChangesAsync();
    }
}