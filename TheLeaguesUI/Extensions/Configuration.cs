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
            #region Configure environment specific settings
            //Not needed to explicitly load in an appsettings.json, this gets done by the Blazor framework. Simply reference it.
            //var UISettings = builder.Configuration.GetSection("UISettings");
            //builder.Services.Configure<UISettings>(UISettings);
            var configBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

            builder.Services.Configure<UISettings>(builder.Configuration.GetSection("UISettings"));

            var awaySettings = builder.Configuration
            .GetSection("UISettings")
            .Get<UISettings>()
            ?? throw new InvalidOperationException("UISettings could not be configured.");
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