using Models.DTO;
using theLeagues.Services.Interfaces;

namespace HomeAPI.Endpoints
{
    public static class PlayerEndpoints
    {
        public static void RegisterPlayerEndpoints(this IEndpointRouteBuilder app)
        {
            // Define a group for player-related endpoints
            var playerEndpointsGroup = app.MapGroup("/players");

            // GET ALL PLAYERS
            playerEndpointsGroup.MapGet("", async (IPlayerService playerService) =>
            {
                var players = await playerService.GetAllPlayersAsync();
                return players != null && players.Any()
                    ? Results.Ok(players)
                    : Results.NotFound("No players found.");
            })
            .WithName("GetAllPlayers")
            .WithDescription("Retrieves all players")
            .WithSummary("This endpoint retrieves a list of all players available in the database.")
            .WithTags("Players")
            .Produces<IEnumerable<PlayerDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // GET PLAYER BY PLATER ID
            playerEndpointsGroup.MapGet("/player-id={id:int}", async (IPlayerService playerService, int id) =>
            {
                var player = await playerService.GetByIdAsync(id);
                return player is not null ? Results.Ok(player) : Results.NotFound($"Player with ID {id} not found.");
            })
            .WithName("GetPlayerById")
            .WithDescription("Retrieves a player by their ID")
            .WithSummary("This endpoint retrieves a specific player by their ID. If the player is not found, it returns a 404 Not Found response.")
            .WithTags("Players")
            .Produces<PlayerDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // GET PLAYERS BY TEAM ID
            playerEndpointsGroup.MapGet("/team-id={teamId:int}", async (IPlayerService playerService, int teamId) =>
            {
                var players = await playerService.GetByTeamIdAsync(teamId);
                return players is not null && players.Any()
                    ? Results.Ok(players)
                    : Results.NotFound($"No players found for team with ID {teamId}.");
            })
            .WithName("GetPlayersByTeamId")
            .WithDescription("Retrieves players by their team ID")
            .WithSummary("This endpoint retrieves all players associated with a specific team ID. If no players are found, it returns a 404 Not Found response.")
            .WithTags("Players")
            .Produces<IEnumerable<PlayerDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // GET PLAYERS BY LEAGUE ID

            playerEndpointsGroup.MapGet("/league-id={leagueId:int}", async (IPlayerService playerService, int leagueId) =>
            {
                var players = await playerService.GetByLeagueIdAsync(leagueId);
                return players is not null && players.Any()
                    ? Results.Ok(players)
                    : Results.NotFound($"No players found for league with ID {leagueId}.");
            })
            .WithName("GetPlayersByLeagueId")
            .WithDescription("Retrieves players by their league ID")
            .WithSummary("This endpoint retrieves all players associated with a specific league ID. If no players are found, it returns a 404 Not Found response.")
            .WithTags("Players")
            .Produces<IEnumerable<PlayerDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            //GET PLAYERS BY COUNTRY
            playerEndpointsGroup.MapGet("/country={country}", async (IPlayerService playerService, string country) =>
            {
                var players = await playerService.GetByCountryAsync(country);
                return players is not null && players.Any()
                    ? Results.Ok(players)
                    : Results.NotFound($"No players found for country {country}.");
            })
            .WithName("GetPlayersByCountry")
            .WithDescription("Retrieves players by their country")
            .WithSummary("This endpoint retrieves all players associated with a specific country. If no players are found, it returns a 404 Not Found response.")
            .WithTags("Players")
            .Produces<IEnumerable<PlayerDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}