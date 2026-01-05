using AwayAPI.BackgroundServices.Interfaces;
using AwayAPI.DAL;
using AwayAPI.Services;
using AwayAPI.Services.Interfaces;

namespace AwayAPI.Endpoints
{
    public static class PlayerEndpoints
    {
        public static void RegisterPlayerEndpoints(this IEndpointRouteBuilder routes)
        {
            var playerEndpointsGroup = routes.MapGroup("/players");

            #region UpdateAllPlayers
                playerEndpointsGroup.MapPost("", async (AwayDBContext awayDbContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider) =>
                {
                    await queue.QueueBackgroundWorkItemAsync(async ct =>
                    {
                        using(var scope = serviceProvider.CreateScope())
                        {
                            var processor = scope.ServiceProvider.GetRequiredService<PlayerService>();
                            await processor.UpdateAllPlayers(ct);
                        }
                    });

                    return Results.Accepted("Job has been scheduled to update all players in database.");
                });
            #endregion

            #region UpdatePlayerById
            #endregion
        }
    }
}