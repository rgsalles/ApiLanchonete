namespace ApiLanchonete.Products
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProduct();
        Task<ProductDto?> GetProductById(Guid id);
        Task<ProductDto> CreateProduct(CreateProductDto dto);
        Task<bool> UpdateProduct(Guid id, UpdateProductDto dto);
        Task<bool> DeleteProduct(Guid id);

    }
}
