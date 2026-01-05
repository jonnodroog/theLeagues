using AwayAPI.BackgroundServices.Interfaces;
using AwayAPI.DAL;
using AwayAPI.Services;
using AwayAPI.Services.Interfaces;

namespace AwayAPI.Endpoints
{
    public static class TeamEndpoints
    {
        public static void RegisterTeamEndpoints(this IEndpointRouteBuilder routes)
        {
            var teamEndpointsGroup = routes.MapGroup("/teams");

            #region UpdateAllTeams
            teamEndpointsGroup.MapPost("", async (AwayDBContext awayDbContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider) =>
            {
                await queue.QueueBackgroundWorkItemAsync(async ct =>
                {
                    using(var scope = serviceProvider.CreateScope())
                    {
                        var processor = scope.ServiceProvider.GetRequiredService<ITeamService>();
                        await processor.UpdateAllTeams(ct);
                    }
                });

                return Results.Accepted("Job has been scheduled to update all teams in database.");
            });
            #endregion

            #region UpdateTeamById
            teamEndpointsGroup.MapPost("/{id:int}", async (AwayDBContext awayDbContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider, int id) =>
            {
                await queue.QueueBackgroundWorkItemAsync(async ct =>
                {
                    using(var scope = serviceProvider.CreateScope())
                    {
                        var processor = scope.ServiceProvider.GetRequiredService<ITeamService>();
                        await processor.UpdateTeamById(id,ct);
                    }
                });

                return Results.Accepted($"Job has been scheduled to update team with id: {id}");
            });
            #endregion
        }
    }
}