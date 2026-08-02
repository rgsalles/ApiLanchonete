using ApiLanchonete.Features.Products;

public interface IProductService
{
    Task<List<ProductDto>> GetProducts();
    Task<ProductDto> GetProductById(Guid id);
    Task<ProductDto> CreateProduct(CreateProductDto dto);
    Task UpdateProduct(Guid id, UpdateProductDto dto);
    Task DeleteProduct(Guid id);
}