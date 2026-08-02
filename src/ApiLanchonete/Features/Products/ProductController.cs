using ApiLanchonete.Features.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        return Ok(await service.GetProducts());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        return Ok(await service.GetProductById(id));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var product = await service.CreateProduct(dto);

        return CreatedAtAction(nameof(GetProduct),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto dto)
    {
        await service.UpdateProduct(id, dto);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await service.DeleteProduct(id);

        return NoContent();
    }
}