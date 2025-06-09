using System;
using AwayAPI.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            #region DAL
            var connectionString = builder.Configuration.GetConnectionString("ConnectionString_Away_Dev");
                builder.Services.AddSqlServer<AwayDBContext>(connectionString);
            #endregion

            #region Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("V1", new OpenApiInfo { Title = "Away API", Description = "API for pulling data from API-Football", Version = "V1" });
                });
            #endregion

            #region CORS
            #endregion

            #region Configuration
                builder.Services.Configure<AwaySettings>(builder.Configuration.GetSection("env"));
            #endregion

            #region Logging
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
            #endregion

            #region Background Service
                builder.Services.AddHostedService<AwayBackgroundService>();
            #endregion
        }

        public static void RegisterMiddleware(this WebApplication app)
        {
            //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Away API");
                });
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();
        }
    }
}