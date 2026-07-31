using ApiLanchonete.DTOs;

namespace ApiLanchonete.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProduct();
        Task<ProductDto?> GetProductById(int id);
        Task<ProductDto> CreateProduct(CreateProductDto dto);
        Task<bool> UpdateProduct(Guid id, UpdateProductDto dto);
        Task<bool> DeleteProduct(int id);

    }
}
