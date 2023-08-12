using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
  public class ProductRepository : IProductRepository
  {
    public ProductRepository(DataContext context)
    {
      _context = context;
    }

    private readonly DataContext _context;

    public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
    {
      return await _context.ProductBrands.ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id) => await _context.Products
          .Include(p => p.ProductType)
      .Include(p => p.ProductBrand)
      .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
      return await _context.Products
          .Include(p => p.ProductType)
          .Include(p => p.ProductBrand)
          .ToListAsync();
    }

    public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
    {
      return await _context.ProductTypes.ToListAsync();
    }

  }
}