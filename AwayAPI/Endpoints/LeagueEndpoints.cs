using System;
using AwayAPI.BackgroundServices.Interfaces;
using AwayAPI.DAL;
using AwayAPI.Services.Interfaces;

namespace AwayAPI.Endpoints
{
    public static class LeagueEndpoints
    {
        public static void RegisterLeagueEndpoints(this IEndpointRouteBuilder routes)
        {
            var leagueEndpointsGroup = routes.MapGroup("/leagues");

            #region UpdateAllLeagues
            leagueEndpointsGroup.MapPost("", async (AwayDBContext awayDbContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider) =>
            {
                await queue.QueueBackgroundWorkItemAsync(async ct =>
                {
                    using(var scope = serviceProvider.CreateScope())
                    {
                        var processor = scope.ServiceProvider.GetRequiredService<ILeagueService>();
                        await processor.UpdateAllLeagues(ct);
                    }
                });

                return Results.Accepted("Job has been scheduled to update all leagues in database.");
            })
            .WithName("UpdateAllLeagues")
            .WithSummary("Updates all leagues")
            .WithDescription("This endpoint updates all leagues available in the database.");
            #endregion

            #region UpdateLeagueById
            leagueEndpointsGroup.MapPost("/{id:int}", async (AwayDBContext awayDbContext, IBackgroundTaskQueue queue, IServiceProvider serviceProvider, int id) =>
            {
                await queue.QueueBackgroundWorkItemAsync(async ct =>
                {
                    using(var scope = serviceProvider.CreateScope())
                    {
                        var processor = scope.ServiceProvider.GetRequiredService<ILeagueService>();
                        await processor.UpdateLeague(id,ct);
                    }
                });

                // Return 202 instead of 200 because work has been accepted and not necessarily completed.
                return Results.Accepted($"Job has been scheduled to update league with ID: {id}");
            })
            .WithName("UpdateLeagueById")
            .WithSummary("Updates the data for a specific league")
            .WithDescription("This endpoint updates a specific league based on the id provided in the request..");
            #endregion 
        }
    }
}