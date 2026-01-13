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
                await queue.QueueBackgroundWorkItemAsync(async (scopedProvider, ct) =>
                {
                    var processor = scopedProvider.GetRequiredService<IPlayerService>();
                    await processor.UpdateAllPlayers(ct);
                });

                return Results.Accepted("Job has been scheduled to update all players in database.");
            });
            #endregion

            #region UpdatePlayerById
            playerEndpointsGroup.MapPost("/{id:int}", async (AwayDBContext awayDbContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider, int id) =>
               {
                   await queue.QueueBackgroundWorkItemAsync(async (scopedProvider, ct) =>
                   {
                       var processor = scopedProvider.GetRequiredService<IPlayerService>();
                       await processor.UpdatePlayerById(id, ct);
                   });

                   return Results.Accepted($"Job has been scheduled to update player with id: {id}.");
               });
            #endregion
        }
    }
}