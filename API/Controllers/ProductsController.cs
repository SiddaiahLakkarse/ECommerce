using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[Controller]")]
  public class ProductsController : ControllerBase
  {
    public IProductRepository _repo { get; }
    public ProductsController(IProductRepository repo)
    {
      _repo = repo;

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsList()
    {
      var products = await _repo.GetProductsAsync();
      return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
      return await _repo.GetProductByIdAsync(id);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
      return Ok(await _repo.GetProductBrandsAsync());
    }


    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
    {
      return Ok(await _repo.GetProductTypesAsync());
    }
  }
}