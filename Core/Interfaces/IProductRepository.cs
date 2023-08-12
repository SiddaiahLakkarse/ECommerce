using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
         Task<Product> GetProductByIdAsync(int Id);

         Task<IReadOnlyList<Product>> GetProductsAsync();

         Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();

         Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
    }
}