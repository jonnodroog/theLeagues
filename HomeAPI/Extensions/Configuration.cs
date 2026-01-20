using System;
using System.Threading.RateLimiting;
using HomeAPI.DAL;
using HomeAPI.Endpoints;
using HomeAPI.Services;
using HomeAPI.Services.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using theLeagues.Services.Interfaces;

namespace HomeAPI.Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            #region Configure environment specific settings
                var configBuilder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange:true)
                            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

                builder.Services.Configure<HomeSettings>(builder.Configuration.GetSection("HomeSettings"));
            #endregion

             #region HTTP Client
            builder.Services.AddHttpClient();
            #endregion
            
            #region DAL
            var host = builder.Configuration["THELEAGUES_DB_HOST"];
            var port = builder.Configuration["THELEAGUES_DB_PORT"];
            var db = builder.Configuration["THELEAGUES_DB_NAME"];
            var user = builder.Configuration["THELEAGUES_ROOT_USER"];
            var pass = builder.Configuration["THELEAGUES_ROOT_PASSWORD"];

            var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass};";
            builder.Services.AddDbContext<HomeDBContext>(options =>
                {   
                    options.UseNpgsql(connectionString);
                });  
            builder.Services.AddHealthChecks()
                            .AddNpgSql(connectionString!, name: "theleagues-db");
            #endregion

            #region Rate limiting

            #endregion

            #region Declare custom services
            builder.Services.AddScoped<ILeagueService, LeagueService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            #endregion

            #region Swagger
            builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddOpenApiDocument(config =>
                {
                    config.DocumentName = "HomeAPI";
                    config.Title = "HomeAPI V1";
                    config.Version = "V1";
                });
            #endregion

            #region Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyHeader());
            });
            #endregion

            #region AuthN/AuthZ
            #endregion
        }
        public static void RegisterMiddlewares(this WebApplication app)
        {
            //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order
            app.UseExceptionHandler("/Error");
            app.UseHttpsRedirection();
            app.UseCors("AllowAnyOrigin");
            //app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi(config =>
                {
                    config.DocumentTitle = "HomeAPI";
                    config.Path = "/swagger";
                    config.DocExpansion = "/swagger/{documentName}/swagger.json";
                    config.DocExpansion = "list";
                });
            }

            app.RegisterLeagueEndpoints();
            app.RegisterPlayerEndpoints();
            app.RegisterTeamEndpoints();
            app.MapHealthChecks("/health");
        }
    }
}