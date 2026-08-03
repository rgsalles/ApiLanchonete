using ApiLanchonete.Common.Exceptions;
using ApiLanchonete.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Features.Products;

public class ProductService(AppDbContext context) : IProductService
{
    public async Task<List<ProductDto>> GetProducts(Guid? companyId = null)
    {
        var query = context.Products.AsNoTracking();

        if (companyId.HasValue)
            query = query.Where(product => product.CompanyId == companyId.Value);

        var products = await query.ToListAsync();

        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> GetProductById(Guid id)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
            throw new NotFoundException($"Product with ID {id} not found.");

        return ToDto(product);
    }

    public async Task<ProductDto> CreateProduct(CreateProductDto dto)
    {
        var companyExists = await context.Companies.AnyAsync(company => company.Id == dto.CompanyId);

        if (!companyExists)
            throw new NotFoundException($"Company with ID {dto.CompanyId} not found.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            Image = dto.Image,
            Active = true,
            AvailableFrom = dto.AvailableFrom,
            AvailableUntil = dto.AvailableUntil,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        };

        context.Products.Add(product);

        await context.SaveChangesAsync();

        return ToDto(product);
    }

    public async Task UpdateProduct(Guid id, UpdateProductDto dto)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
            throw new NotFoundException($"Product with ID {id} not found.");

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.Image = dto.Image;
        product.Active = dto.Active;
        product.AvailableFrom = dto.AvailableFrom;
        product.AvailableUntil = dto.AvailableUntil;

        product.UpdatedAt = DateTime.UtcNow;
        product.UpdatedBy = "System";

        await context.SaveChangesAsync();
    }

    public async Task DeleteProduct(Guid id)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
            throw new NotFoundException($"Product with ID {id} not found.");

        context.Products.Remove(product);

        await context.SaveChangesAsync();
    }

    private static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            CompanyId = product.CompanyId,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Image = product.Image,
            Active = product.Active,
            AvailableFrom = product.AvailableFrom,
            AvailableUntil = product.AvailableUntil,
            CreatedAt = product.CreatedAt,
            CreatedBy = product.CreatedBy,
            UpdatedAt = product.UpdatedAt,
            UpdatedBy = product.UpdatedBy
        };
    }
}
