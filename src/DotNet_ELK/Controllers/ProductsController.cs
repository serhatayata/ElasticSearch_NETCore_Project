using DotNet_ELK.Models;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace DotNet_ELK.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController:ControllerBase
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IElasticClient elasticClient, ILogger<ProductsController> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }


    [HttpGet(Name="GetProducts")]
    public async Task<IActionResult> Get(string keyword)
    {
        var results = await _elasticClient.SearchAsync<Product>(
            s=>s.Query(
                q => q.QueryString(
                    d=>d.Query('*'+keyword+'*')
                )
            ).Size(1000)
        );

        return Ok(results.Documents.ToList());
    }

    [HttpPost(Name="AddProduct")]
    public async Task<IActionResult> Post(Product product)
    {
        await _elasticClient.IndexDocumentAsync(product);
        return Ok();
    }

}
