using TheLeaguesCMS.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookies(options =>
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

        var email = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if(!string.Equals(email, allowedEmail, StringComparison.OrdinalIgnoreCase))
        {
            context.Fail("Unauthorised email address");
        }

        return Task.CompletedTask;
    };
});
builder.AddAuthorization();

builder.AddCascadingAuthenticationState();
builder.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
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
    new Microsoft.AspNetCore.Authentication.AuthenticationProperties{RedirectUri="/"}, [GoogleDefaults.AuthenticationScheme]));

app.MapGet("/logout", async (HttpContext context) => 
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).RequireAuthorization();

app.MapGet("/access-denied", () => Results.Text("Access denied"));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
