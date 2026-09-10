using TheLeaguesCMS.Components;
using TheLeaguesCMS.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterServices();
var app = builder.Build();

app.RegisterMiddlewares();
app.Run();
