using System.Text.Json.OptionalValue;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		// ⭐ ONE-LINE SETUP - This enables OptionalValue<T> for all models!
		// No need for [JsonConverter] attributes on properties anymore
		options.JsonSerializerOptions.AddOptionalValueSupport();

		options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.WriteIndented = true;
	});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "GlobalRegistrationApi v1");
		options.RoutePrefix = string.Empty;
	});
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
