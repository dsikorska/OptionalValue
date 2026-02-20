using ExampleApi.Models;

namespace ExampleApi.Services;

public class ProductService : IProductService
{
	private readonly List<Product> _products = new()
	{
		new Product
		{
			Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
			Name = "Laptop",
			Description = "High-performance laptop",
			Price = 999.99m,
			Stock = 50,
			Category = "Electronics",
			CreatedAt = DateTime.UtcNow.AddDays(-60),
			UpdatedAt = DateTime.UtcNow.AddDays(-60)
		},
		new Product
		{
			Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
			Name = "Mouse",
			Description = null,
			Price = 29.99m,
			Stock = 200,
			Category = "Accessories",
			CreatedAt = DateTime.UtcNow.AddDays(-45),
			UpdatedAt = DateTime.UtcNow.AddDays(-45)
		}
	};

	public IEnumerable<Product> GetAll() => _products;

	public Product? GetById(Guid id) => _products.FirstOrDefault(p => p.Id == id);

	public Product Create(Product product)
	{
		product.Id = Guid.NewGuid();
		product.CreatedAt = DateTime.UtcNow;
		product.UpdatedAt = DateTime.UtcNow;
		_products.Add(product);
		return product;
	}

	public Product? Update(Guid id, PatchProductRequest request)
	{
		var product = GetById(id);
		if (product == null)
		{
			return null;
		}

		// OptionalValue<T> pattern - only update specified fields

		if (request.Name.IsSpecified)
		{
			product.Name = request.Name.Value ?? string.Empty;
		}

		if (request.Description.IsSpecified)
		{
			product.Description = request.Description.Value; // Can be null
		}

		if (request.Price.IsSpecified && request.Price.Value.HasValue)
		{
			product.Price = request.Price.Value.Value;
		}

		if (request.Stock.IsSpecified && request.Stock.Value.HasValue)
		{
			product.Stock = request.Stock.Value.Value;
		}

		if (request.Category.IsSpecified)
		{
			product.Category = request.Category.Value; // Can be null
		}

		product.UpdatedAt = DateTime.UtcNow;

		return product;
	}

	public bool Delete(Guid id)
	{
		var product = GetById(id);
		if (product == null)
		{
			return false;
		}

		_products.Remove(product);
		return true;
	}
}
