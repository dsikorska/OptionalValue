var builder = WebApplication.CreateBuilder(args);

// Standard controller setup — NO AddOptionalValueSupport() call!
// OptionalValue<T> works here via [JsonConverter] attributes on individual properties.
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
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
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "PerPropertyAttributeApi v1");
		options.RoutePrefix = string.Empty;
	});
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
