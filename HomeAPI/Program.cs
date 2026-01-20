using HomeAPI.DAL;
using HomeAPI.Extensions;

// Loads environment variables
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.RegisterMiddlewares();

app.MapGet("/", () => "Home API is live.");

app.Run();
