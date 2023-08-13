using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;

namespace API.Controllers
{
  [ApiController]
  [Route("api/[Controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;
    private readonly IGenericRepository<Product> _productsRepo;
    //private readonly IMapper _mapper;
    public ProductsController(IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IGenericRepository<ProductBrand> productBrandRepo)
    {
      _productsRepo = productsRepo;
      _productTypeRepo = productTypeRepo;
      _productBrandRepo = productBrandRepo;

    }

    [HttpGet]
    public async Task<ActionResult<List<ProductToReturnDto>>> GetProductsList()
    {
      var spec = new ProductsWithTypesAndBrandsSpecification();

      var products = await _productsRepo.ListAsync(spec);

      return products.Select(product => new ProductToReturnDto
      {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        PictureUrl = product.PictureUrl,
        Price = product.Price,
        ProductBrand = product.ProductBrand.Name,
        ProductType = product.ProductType.Name

      }).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
    {
      var spec = new ProductsWithTypesAndBrandsSpecification(id);

      var product = await _productsRepo.GetEntityWithSpec(spec);

      return new ProductToReturnDto
      {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        PictureUrl = product.PictureUrl,
        Price = product.Price,
        ProductBrand = product.ProductBrand.Name,
        ProductType = product.ProductType.Name

      };
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
      return Ok(await _productsRepo.ListAllAsync());
    }


    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
    {
      return Ok(await _productsRepo.ListAllAsync());
    }
  }
}