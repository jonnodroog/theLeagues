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

            #region GetAllPlayers
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
            #endregion

            #region GetPlayerByPlayerId
            playerEndpointsGroup.MapGet("/player-id={id:int}", async (IPlayerService playerService, int id) =>
            {
                var player = await playerService.GetPlayerByPlayerIdAsync(id);
                return player is not null ? Results.Ok(player) : Results.NotFound($"Player with ID {id} not found.");
            })
            .WithName("GetPlayerById")
            .WithDescription("Retrieves a player by their ID")
            .WithSummary("This endpoint retrieves a specific player by their ID. If the player is not found, it returns a 404 Not Found response.")
            .WithTags("Players")
            .Produces<PlayerDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
            #endregion

            #region GetPlayersByTeamId
            playerEndpointsGroup.MapGet("/team-id={teamId:int}", async (IPlayerService playerService, int teamId) =>
            {
                var players = await playerService.GetPlayersByTeamIdAsync(teamId);
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
            #endregion

            #region GetPlayersByLeagueId
            playerEndpointsGroup.MapGet("/league-id={leagueId:int}", async (IPlayerService playerService, int leagueId) =>
            {
                var players = await playerService.GetPlayersByLeagueIdAsync(leagueId);
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
            #endregion
        }
    }
}