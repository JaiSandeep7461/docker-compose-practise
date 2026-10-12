
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api", () => new
{
    message = "Hello from .NET Backend API!"
});

app.MapGet("/", () => "Backend is running!");

app.Run();
