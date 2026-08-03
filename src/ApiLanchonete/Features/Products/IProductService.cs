using ApiLanchonete.Features.Products;

public interface IProductService
{
    Task<List<ProductDto>> GetProducts(Guid? companyId = null);
    Task<ProductDto> GetProductById(Guid id);
    Task<ProductDto> CreateProduct(CreateProductDto dto);
    Task UpdateProduct(Guid id, UpdateProductDto dto);
    Task DeleteProduct(Guid id);
}
