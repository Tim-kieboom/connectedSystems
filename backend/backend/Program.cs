using backend;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<UpdatePositions>();
var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "WebContent")),
    RequestPath = ""
});

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
