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
            #region DAL
                builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("HomeDbConnection")));
            #endregion

            #region Configure environment specific settings
                var configBuilder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange:true)
                            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                            .Build();

                builder.Services.Configure<HomeSettings>(builder.Configuration.GetSection("HomeSettings"));
            #endregion

            #region Rate limiting
                builder.Services.AddRateLimiter(_ => _
                .AddFixedWindowLimiter(policyName: "fixed", options =>
                {
                    options.PermitLimit = 4;
                    options.Window = TimeSpan.FromSeconds(12);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 10;
                }));
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
                                      .AllowCredentials()
                                      .AllowAnyHeader());
            });
            #endregion

            #region AuthN/AuthZ
            #endregion
        }
        public static void RegisterMiddlewares(this WebApplication app)
        {
                app.UseHttpsRedirection();

                app.UseCors("AllowAnyOrigin");

                app.UseExceptionHandler();
                //app.UseAuthorization();
            
                if(app.Environment.IsDevelopment())
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

                app.UseRateLimiter();

                app.RegisterLeagueEndpoints();
        }
    }
}