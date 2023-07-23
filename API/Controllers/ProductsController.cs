using Infrasturucture.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[Controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly DataContext _context;
    public ProductsController(DataContext context)
    {
      _context = context;

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsList()
    {
        var products = await _context.Products.ToListAsync();
        return products;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
      return await _context.Products.FindAsync(id);
    }
  }
}