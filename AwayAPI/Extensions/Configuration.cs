using System;
using System.Net.Http.Headers;
using AwayAPI.DAL;
using Microsoft.EntityFrameworkCore;

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
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();
        }
    }
}