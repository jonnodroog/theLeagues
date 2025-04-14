using System;

namespace HomeAPI.LeagueEndpoints
{
    public static class LeagueEndpoints
    {
        public static void RegisterLeagueEndpoints(this IEndpointRouteBuilder routes)
        {
            var leagueEndpointsGroup = routes.MapGroup("/leagues");

            leagueEndpointsGroup.MapGet("", async () =>
            {

            });
        }
    }
}