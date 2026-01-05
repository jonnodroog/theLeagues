namespace AwayAPI.Services.Interfaces
{
    public interface ILeagueService
    {
        public Task UpdateAllLeagues(CancellationToken ct);
        public Task UpdateLeague(int id, CancellationToken ct);
    }
}