using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Orders;

public class OrderService(AppDbContext context) : IOrderService
{
    public async Task<List<OrderDto>> GetOrders()
        => await context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .OrderByDescending(order => order.CreatedAt)
            .Select(order => ToDto(order))
            .ToListAsync();

    public async Task<OrderDto> GetOrderById(Guid id)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(order => order.Id == id)
            ?? throw new NotFoundException($"Order with ID {id} not found.");

        return ToDto(order);
    }

    public async Task<OrderDto> CreateOrder(CreateOrderDto dto)
    {
        var branch = await context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(branch => branch.Id == dto.BranchId)
            ?? throw new NotFoundException($"Branch with ID {dto.BranchId} not found.");

        if (!branch.Active)
            throw new BadRequestException("Orders cannot be created for an inactive branch.");

        var client = await context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(client => client.Id == dto.ClientId)
            ?? throw new NotFoundException($"Client with ID {dto.ClientId} not found.");

        if (client.CompanyId != branch.CompanyId)
            throw new BadRequestException("The client must belong to the same company as the branch.");

        var productIds = dto.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await context.Products
            .Where(product => product.CompanyId == branch.CompanyId && productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);

        if (products.Count != productIds.Count)
            throw new BadRequestException("Every product must belong to the same company as the branch.");

        var now = DateTime.UtcNow;
        foreach (var item in dto.Items)
        {
            var product = products[item.ProductId];
            if (!product.Active || product.AvailableFrom > now ||
                (product.AvailableUntil.HasValue && product.AvailableUntil.Value < now))
                throw new BadRequestException($"Product '{product.Name}' is unavailable.");
        }

        var inventoryByProduct = await context.Inventories
            .Where(inventory => inventory.BranchId == branch.Id && productIds.Contains(inventory.ProductId))
            .ToDictionaryAsync(inventory => inventory.ProductId);

        foreach (var item in dto.Items)
        {
            if (!inventoryByProduct.TryGetValue(item.ProductId, out var inventory) || !inventory.Active)
                throw new BadRequestException($"Product '{products[item.ProductId].Name}' is not available at this branch.");

            var orderedQuantity = dto.Items
                .Where(orderItem => orderItem.ProductId == item.ProductId)
                .Sum(orderItem => orderItem.Quantity);

            if (inventory.Quantity < orderedQuantity)
                throw new BadRequestException($"Insufficient stock for product '{products[item.ProductId].Name}'.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            BranchId = branch.Id,
            ClientId = client.Id,
            Status = OrderStatus.Pending,
            StockReserved = true,
            CreatedAt = now,
            CreatedBy = "System"
        };

        foreach (var productId in productIds)
        {
            var quantity = dto.Items.Where(item => item.ProductId == productId).Sum(item => item.Quantity);
            var product = products[productId];
            var inventory = inventoryByProduct[productId];
            var totalPrice = product.Price * quantity;

            inventory.Quantity -= quantity;
            inventory.UpdatedAt = now;
            inventory.UpdatedBy = "Order";

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price,
                TotalPrice = totalPrice
            });
            order.TotalAmount += totalPrice;
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return ToDto(order, products);
    }

    public async Task UpdateOrder(Guid id, UpdateOrderDto dto)
    {
        var order = await context.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id)
            ?? throw new NotFoundException($"Order with ID {id} not found.");

        if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            throw new BadRequestException("A delivered or cancelled order cannot be changed.");

        if (!IsValidTransition(order.Status, dto.Status))
            throw new BadRequestException("Invalid order status transition.");

        if (dto.Status == OrderStatus.Cancelled && order.StockReserved)
        {
            var productIds = order.Items.Select(item => item.ProductId).ToList();
            var inventoryByProduct = await context.Inventories
                .Where(inventory => inventory.BranchId == order.BranchId && productIds.Contains(inventory.ProductId))
                .ToDictionaryAsync(inventory => inventory.ProductId);

            foreach (var item in order.Items)
            {
                if (!inventoryByProduct.TryGetValue(item.ProductId, out var inventory))
                    throw new BadRequestException("The order inventory entry was not found.");

                inventory.Quantity += item.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;
                inventory.UpdatedBy = "Order cancellation";
            }

            order.StockReserved = false;
        }

        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = "System";
        await context.SaveChangesAsync();
    }

    public async Task DeleteOrder(Guid id)
    {
        var order = await context.Orders.FindAsync(id)
            ?? throw new NotFoundException($"Order with ID {id} not found.");

        if (order.Status != OrderStatus.Pending)
            throw new BadRequestException("Only pending orders can be deleted. Cancel the order to return stock.");

        await UpdateOrder(id, new UpdateOrderDto { Status = OrderStatus.Cancelled });
        context.Orders.Remove(order);
        await context.SaveChangesAsync();
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next) =>
        (current, next) switch
        {
            (OrderStatus.Pending, OrderStatus.Preparing) => true,
            (OrderStatus.Preparing, OrderStatus.Ready) => true,
            (OrderStatus.Ready, OrderStatus.Delivered) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Preparing, OrderStatus.Cancelled) => true,
            (OrderStatus.Ready, OrderStatus.Cancelled) => true,
            _ => false
        };

    private static OrderDto ToDto(
        Order order,
        IReadOnlyDictionary<Guid, ApiLanchonete.Features.Products.Product>? products = null) => new()
    {
        Id = order.Id,
        BranchId = order.BranchId,
        ClientId = order.ClientId,
        Status = order.Status,
        TotalAmount = order.TotalAmount,
        CreatedAt = order.CreatedAt,
        CreatedBy = order.CreatedBy,
        UpdatedAt = order.UpdatedAt,
        UpdatedBy = order.UpdatedBy,
        Items = order.Items.Select(item => new OrderItemDto
        {
            ProductId = item.ProductId,
            ProductName = products is not null ? products[item.ProductId].Name : item.Product.Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        }).ToList()
    };
}
