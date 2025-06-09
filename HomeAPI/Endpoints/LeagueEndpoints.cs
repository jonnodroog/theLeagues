using System;
using HomeAPI.Services.Interfaces;

namespace HomeAPI.Endpoints
{
    public static class LeagueEndpoints
    {
        public static void RegisterLeagueEndpoints(this IEndpointRouteBuilder routes)
        {
            var leagueEndpointsGroup = routes.MapGroup("/leagues");

            leagueEndpointsGroup.MapGet("", async (ILeagueService leagueService) =>
            {
                var leagues = await leagueService.GetAllLeaguesAsync();
                return Results.Ok(leagues);
            })
            .WithName("GetAllLeagues")
            .WithSummary("Retrieves all leagues")
            .WithDescription("This endpoint retrieves a list of all leagues available in the database.");

            leagueEndpointsGroup.MapGet("/{id:int}", async (ILeagueService leagueService, int id) =>
            {
                var league = await leagueService.GetLeagueByIdAsync(id);

                return league is not null ? Results.Ok(league) : Results.NotFound($"League with ID {id} not found.");
            })
            .WithName("GetLeagueById")
            .WithSummary("Retrieves a league by its ID")
            .WithDescription("This endpoint retrieves a specific league by its ID. If the league is not found, it returns a 404 Not Found response.");
        }
    }
}