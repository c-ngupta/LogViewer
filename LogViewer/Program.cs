using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "LogViewer API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "LogViewer API v1");
	});
}

app.UseHttpsRedirection();

// Configure static file serving
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".log"] = "text/plain";

app.UseStaticFiles(new StaticFileOptions
{
	ContentTypeProvider = provider
});

app.UseAuthorization();
app.MapControllers();

provider.Mappings[".log"] = "text/plain";

app.UseStaticFiles(new StaticFileOptions
{
	ContentTypeProvider = provider,
	// Optional: Serve files from root if needed (not recommended for production)
	// FileProvider = new PhysicalFileProvider(
	//     Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
	// RequestPath = ""
});

app.Run();