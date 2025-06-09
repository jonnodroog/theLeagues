using System.Collections.Generic;
using System.Threading.Tasks;
using HomeAPI.Services;
using HomeAPI.Services.Interfaces;
using Models.DTO;

namespace HomeAPI.Endpoints
{
    public static class TemEndpoints
    {
        public static void RegisterTeamEndpoints(this IEndpointRouteBuilder app)
        {
            var teamEndpointsGroup = app.MapGroup("/team");


            // GET ALL TEAMS
            teamEndpointsGroup.MapGet("", async (ITeamService teamService) =>
            {
                var teams = await teamService.GetAllTeamsAsync();

                return teams != null && teams.Any()
                    ? Results.Ok(teams)
                    : Results.NotFound("No teams found");
            })
            .WithName("GetAllTeams")
            .WithDescription("Retrieves all teams in all the leagues")
            .WithSummary("This endpoint retrieves a list of all teams in the database")
            .Produces<IEnumerable<TeamDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // GET TEAM BY TEAM ID
            teamEndpointsGroup.MapGet("/team-id={id:int}", async (ITeamService teamService, int id) =>
            {
                var team = await teamService.GetByIdAsync(id);

                return team != null ? Results.Ok(team) : Results.NotFound($"Team with ID = {id} not found");
            })
            .WithName("GetTeamById")
            .WithSummary("Retrieves team by their ID")
            .WithDescription("This endpoint retrieves team data based on the specified team ID. If no team data is found it returns 404 Not Found response")
            .Produces<TeamDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // GET TEAMS BY LEAGUE ID
            teamEndpointsGroup.MapGet("/league-id={id:int}", async (ITeamService teamService, int id) =>
            {
                var teams = await teamService.GetByLeagueIdAsync(id);

                return teams != null && teams.Any()
                ? Results.Ok(teams)
                : Results.NotFound($"Teams with league ID = {id} not found");
            })
            .WithName("GetByLeagueId")
            .WithSummary("Retrieves teams by their league ID")
            .WithDescription("This endpoint retrieves team data based on the specified league ID. If no team data is found it returns 404 Not Found response")
            .Produces<IEnumerable<TeamDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
            
            // GET TEAMS BY COUNTRY
            teamEndpointsGroup.MapGet("/country={country:alpha}", async (ITeamService teamService, string country) =>
            {
                var teams = await teamService.GetByCountryAsync(country);

                return teams != null && teams.Any()
                ? Results.Ok(teams) 
                : Results.NotFound($"Teams from {country} not found");
            })
            .WithName("GetTeamsByCountry")
            .WithSummary("Retrieves teams by their country")
            .WithDescription("This endpoint retrieves team data based on the specified country. If no team data is found it returns 404 Not Found response")
            .Produces<IEnumerable<TeamDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}