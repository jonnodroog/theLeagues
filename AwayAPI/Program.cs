using Extensions;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.RegisterMiddleware();

app.MapGet("/", () => "Away API is live lelelele.");

app.Run();
