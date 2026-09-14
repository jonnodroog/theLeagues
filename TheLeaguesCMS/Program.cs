using TheLeaguesCMS.Components;
using TheLeaguesCMS.Extensions;

// Loads environment variables
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.RegisterServices();
var app = builder.Build();

app.RegisterMiddlewares();
app.Run();
