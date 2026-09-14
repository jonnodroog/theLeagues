using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using TheLeaguesCMS.Components;
using TheLeaguesCMS.DAL;
using TheLeaguesCMS.Extensios;
using TheLeaguesCMS.Services;
using TheLeaguesCMS.Services.Interfaces;

namespace TheLeaguesCMS.Extensions
{
    public static class Configuration
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

                #region Configure environment specific settings
                var configBuilder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange:true)
                            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);

                builder.Services.Configure<TheLeaguesCMSSettings>(builder.Configuration.GetSection("TheLeaguesCMSSettings"));
            #endregion

            #region AuthN/AuthZ
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/access-denied";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.SlidingExpiration = true;
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["ThirdPartyAuthentication:Google:ClientId"] ?? throw new InvalidOperationException("Google client ID is not configured.");
                options.ClientSecret = builder.Configuration["ThirdPartyAuthentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google client secret is not configured.");

                options.Events.OnTicketReceived = context =>
                {
                    var allowedEmail = builder.Configuration["ThirdPartyAuthentication:Google:AllowedEmail"] ?? throw new InvalidOperationException("Allowed email is not configured");
                    if(String.IsNullOrWhiteSpace(allowedEmail))
                    {
                        throw new InvalidOperationException("AllowedEmail is not configured.");
                    }

                    var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                    if (!string.Equals(email, allowedEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Fail("Unauthorised email address");
                        context.Response.Redirect("/access-denied");
                        context.HandleResponse();
                    }

                    return Task.CompletedTask;
                };
            });

            builder.Services.AddAuthorization();

            #endregion

            #region DAL
            var host = builder.Configuration["THELEAGUES_DB_HOST"];
            var port = builder.Configuration["THELEAGUES_DB_PORT"];
            var db = builder.Configuration["THELEAGUES_DB_NAME"];
            var user = builder.Configuration["THELEAGUES_ROOT_USER"];
            var pass = builder.Configuration["THELEAGUES_ROOT_PASSWORD"];

            var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass};";
            builder.Services.AddDbContextFactory<TheLeaguesCMSDBContext>(options =>
                {   
                    options.UseNpgsql(connectionString);
                });  
            builder.Services.AddHealthChecks()
                            .AddNpgSql(connectionString!, name: "theleagues-db");
            #endregion

            #region Declare custom services
            builder.Services.AddScoped<ILeagueService, LeagueService>();
            builder.Services.AddScoped<ITeamService, TeamService>();
            builder.Services.AddScoped<IPlayerService, PlayerService>();
            #endregion
            
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddHttpContextAccessor();
        }
        public static void RegisterMiddlewares(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapGet("/login", () => Results.Challenge(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/" }, [GoogleDefaults.AuthenticationScheme]));

            app.MapGet("/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/login");
            }).RequireAuthorization();

            app.MapGet("/access-denied", () => Results.Text("Access denied"));

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
        }
    }
}