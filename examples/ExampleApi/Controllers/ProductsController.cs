using ExampleApi.Models;
using ExampleApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
	private readonly IProductService _productService;
	private readonly ILogger<ProductsController> _logger;

	public ProductsController(IProductService productService, ILogger<ProductsController> logger)
	{
		_productService = productService;
		_logger = logger;
	}

	/// <summary>
	/// Get all products.
	/// </summary>
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
	public IActionResult GetAll()
	{
		var products = _productService.GetAll();
		return Ok(products);
	}

	/// <summary>
	/// Get a product by ID.
	/// </summary>
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(Guid id)
	{
		var product = _productService.GetById(id);
		if (product == null)
		{
			return NotFound(new { message = $"Product with ID {id} not found" });
		}

		return Ok(product);
	}

	/// <summary>
	/// Partially update a product using PATCH.
	/// Only the fields provided in the request will be updated.
	/// </summary>
	/// <remarks>
	/// This endpoint demonstrates the OptionalValue&lt;T&gt; pattern with PER-CLASS ATTRIBUTE ([OptionalValueContract]).
	///
	/// Example requests:
	///
	/// 1. Update only price:
	/// ```json
	/// {
	///   "price": 899.99
	/// }
	/// ```
	///
	/// 2. Clear description (set to null):
	/// ```json
	/// {
	///   "description": null
	/// }
	/// ```
	///
	/// 3. Update multiple fields:
	/// ```json
	/// {
	///   "name": "Gaming Laptop",
	///   "description": "High-end gaming laptop with RTX 4090",
	///   "price": 2499.99,
	///   "stock": 25,
	///   "category": "Gaming"
	/// }
	/// ```
	/// </remarks>
	[HttpPatch("{id}")]
	[ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Patch(Guid id, [FromBody] PatchProductRequest request)
	{
		_logger.LogInformation("PATCH /api/products/{Id} - Received request", id);

		// Log which fields were provided
		_logger.LogInformation(
			"Fields specified: Name={NameSpecified}, Description={DescriptionSpecified}, Price={PriceSpecified}, Stock={StockSpecified}, Category={CategorySpecified}",
			request.Name.IsSpecified,
			request.Description.IsSpecified,
			request.Price.IsSpecified,
			request.Stock.IsSpecified,
			request.Category.IsSpecified
		);

		var product = _productService.Update(id, request);
		if (product == null)
		{
			return NotFound(new { message = $"Product with ID {id} not found" });
		}

		return Ok(product);
	}

	/// <summary>
	/// Delete a product.
	/// </summary>
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Delete(Guid id)
	{
		var deleted = _productService.Delete(id);
		if (!deleted)
		{
			return NotFound(new { message = $"Product with ID {id} not found" });
		}

		return NoContent();
	}
}
