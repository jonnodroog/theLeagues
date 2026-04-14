using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Models;
using TheLeaguesUI.Services;
using TheLeaguesUI.Services.Interfaces;
using TheLeaguesUI.Services.Interfaces.Utilities;

namespace Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebAssemblyHostBuilder builder)
        {
            Console.Write("HELOOOOOOOOOOOOOOOOO");
            #region Configure environment specific settings
            //Not needed to explicitly load in an appsettings.json, this gets done by the Blazor framework. Simply reference it.
            //var UISettings = builder.Configuration.GetSection("UISettings");
            //builder.Services.Configure<UISettings>(UISettings);
            #endregion

            #region HTTP Client
            builder.Services.AddHttpClient("HomeAPIClient",client => 
                {
                    client.BaseAddress = new Uri(builder.Configuration["UISettings:HomeURL"] ?? "http://localhost:5001");
                    // lient.BaseAddress = new Uri("https://externalapi.com");
                });
            #endregion

            #region Custom Services
            builder.Services.AddScoped<IClipboardService, ClipboardService>();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

            builder.Services.AddScoped<ILeagueService, LeagueService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            //builder.Services.AddScoped<IPlayerService, PlayerService>();
            #endregion
        }
    }
}