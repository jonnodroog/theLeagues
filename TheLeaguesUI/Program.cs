using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheLeaguesUI;
using TheLeaguesUI.Services;
using TheLeaguesUI.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

#region Custom Services
builder.Services.AddScoped<IClipboardService, ClipboardService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<ITeamService, TeamService>();
#endregion

#region Http Client
builder.Services.AddScoped(sp => 
    new HttpClient
    { 
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
    });
#endregion

await builder.Build().RunAsync();
