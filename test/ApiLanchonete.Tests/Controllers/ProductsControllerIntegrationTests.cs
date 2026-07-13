using System.Net;
using System.Net.Http.Json;
using ApiLanchonete.Models;
using ApiLanchonete.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiLanchonete.Tests.Controllers;

public class ProductsControllerIntegrationTests
    : IClassFixture<ApiLanchoneteFactory>
{
    private readonly HttpClient client;

    public ProductsControllerIntegrationTests(ApiLanchoneteFactory factory)
    {
        client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task PostEGet_DevemGravarEConsultarProdutoNoSqlServer()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = $"Produto teste integracao {Guid.NewGuid():N}",
            Description = "Registro persistido pelo teste automatizado",
            Image = "produto-teste.png",
            Price = 19.90m,
            Active = true,
            AvailableFrom = DateTime.UtcNow
        };

        var postResponse =
            await client.PostAsJsonAsync("/api/Products", product);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var createdProduct =
            await postResponse.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(createdProduct);
        Assert.Equal(product.Id, createdProduct.Id);

        var getResponse =
            await client.GetAsync($"/api/Products/{product.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var productFromDatabase =
            await getResponse.Content.ReadFromJsonAsync<Product>();
        Assert.NotNull(productFromDatabase);
        Assert.Equal(product.Id, productFromDatabase.Id);
        Assert.Equal(product.Name, productFromDatabase.Name);
        Assert.Equal(product.Price, productFromDatabase.Price);
    }
}
