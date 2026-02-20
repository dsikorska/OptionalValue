using ExampleApi.Models;

namespace ExampleApi.Services;

public interface IProductService
{
	IEnumerable<Product> GetAll();
	Product? GetById(Guid id);
	Product Create(Product product);
	Product? Update(Guid id, PatchProductRequest request);
	bool Delete(Guid id);
}
