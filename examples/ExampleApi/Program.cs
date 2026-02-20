using ExampleApi.Services;
using System.Text.Json.OptionalValue;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		// ⭐ ONE-LINE SETUP - This enables OptionalValue<T> for all models!
		// No need for [JsonConverter] attributes on properties anymore
		options.JsonSerializerOptions.AddOptionalValueSupport();

		// Optional: Configure JSON serialization
		options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.WriteIndented = true;
	});

// Register application services (in-memory for demo)
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IProductService, ProductService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "OptionalValue Example API v1");
		options.RoutePrefix = string.Empty; // Swagger UI at root
	});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
	status = "Healthy",
	timestamp = DateTime.UtcNow,
	message = "OptionalValue Example API is running"
}))
.WithName("HealthCheck");

// Info endpoint to show example usage
app.MapGet("/", () => Results.Ok(new
{
	message = "OptionalValue Example API",
	documentation = "/swagger",
	endpoints = new
	{
		users = new
		{
			getAll = "GET /api/users",
			getById = "GET /api/users/{id}",
			patch = "PATCH /api/users/{id}",
			delete = "DELETE /api/users/{id}",
			exampleIds = new[]
			{
				"11111111-1111-1111-1111-111111111111",
				"22222222-2222-2222-2222-222222222222"
			}
		},
		products = new
		{
			getAll = "GET /api/products",
			getById = "GET /api/products/{id}",
			patch = "PATCH /api/products/{id}",
			delete = "DELETE /api/products/{id}",
			exampleIds = new[]
			{
				"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
				"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
			}
		}
	}
}))
.WithName("Info")
.ExcludeFromDescription();

app.Run();
