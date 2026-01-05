using System;
using System.Net.Http.Headers;
using AwayAPI.BackgroundServices;
using AwayAPI.BackgroundServices.Interfaces;
using AwayAPI.DAL;
using AwayAPI.Endpoints;
using AwayAPI.Services;
using AwayAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            #region Configure environment specific settings
            var configBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

            builder.Services.Configure<AwaySettings>(builder.Configuration.GetSection("AwaySettings"));

            var awaySettings = builder.Configuration
            .GetSection("AwaySettings")
            .Get<AwaySettings>()
            ?? throw new InvalidOperationException("AwaySettings could not be configured.");
            #endregion

            #region HTTP Client
            builder.Services.AddHttpClient("apisports-football", configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://v3.football.api-sports.io");
                configureClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(awaySettings.API_Host, awaySettings.API_Key);
            }
            );
            #endregion
            #region DAL
            var connectionString = builder.Configuration.GetConnectionString("ConnectionString_Away_Dev");
            builder.Services.AddSqlServer<AwayDBContext>(connectionString);    
            #endregion

            #region Background Queue
            builder.Services.AddSingleton<IBackgroundTaskQueue>(new DefaultBackgroundTaskQueue(100));
            #endregion

            #region Background Worker Service
            builder.Services.AddHostedService<QueuedHostedService>();
            #endregion

            #region Custom Services
            builder.Services.AddScoped<ILeagueService, LeagueService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            #endregion

            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddOpenApiDocument(config =>
                {
                    config.DocumentName = "AwayAPI";
                    config.Title = "AwayAPI V1";
                    config.Version = "V1";
                });
            #endregion

            #region CORS
            #endregion

            #region Logging
            #endregion
        }

        public static void RegisterMiddleware(this WebApplication app)
        {
            //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order
            app.UseExceptionHandler("/Error");
            app.UseHttpsRedirection();

            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi(config =>
                {
                    config.DocumentTitle = "AwayAPI";
                    config.Path = "/swagger";
                    config.DocExpansion = "/swagger/{documentName}/swagger.json";
                    config.DocExpansion = "list";
                });
            }

            app.RegisterLeagueEndpoints();
            app.RegisterTeamEndpoints();
            app.RegisterTeamEndpoints();
        }
    }
}