using Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterServices();

var app = builder.Build();
app.RegisterMiddleware();

app.MapGet("/", () => "Away API is live.");

app.Run();
