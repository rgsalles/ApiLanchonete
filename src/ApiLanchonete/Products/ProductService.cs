using ApiLanchonete.Data;
using ApiLanchonete.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiLanchonete.Products;

public class ProductService(AppDbContext context) : IProductService
{
    public async Task<List<ProductDto>> GetProduct()
    {
        return await context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Image = p.Image,
                Active = p.Active,
                AvailableFrom = p.AvailableFrom,
                AvailableUntil = p.AvailableUntil
            })
            .ToListAsync();
    }

    public async Task<ProductDto?> GetProductById(Guid id)
    {
        var product = await context.Products.FindAsync(id);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Image = product.Image,
            Active = product.Active,
            AvailableFrom = product.AvailableFrom,
            AvailableUntil = product.AvailableUntil
        };
    }

    public async Task<ProductDto> CreateProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            Image = dto.Image,
            Active = true,
            AvailableFrom = dto.AvailableFrom,
            AvailableUntil = dto.AvailableUntil
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Image = product.Image,
            Active = product.Active,
            AvailableFrom = product.AvailableFrom,
            AvailableUntil = product.AvailableUntil
        };
    }

    public async Task<bool> UpdateProduct(Guid id, UpdateProductDto dto)
    {
        var product = await context.Products.FindAsync(id);

        if (product == null)
            return false;

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Description = dto.Description;
        product.Image = dto.Image;
        product.Active = dto.Active;
        product.AvailableFrom = dto.AvailableFrom;
        product.AvailableUntil = dto.AvailableUntil;

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProduct(Guid id)
    {
        var product = await context.Products.FindAsync(id);

        if (product == null)
            return false;

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return true;
    }
}