namespace AwayAPI.Services.Interfaces
{
    public interface ILeagueService
    {
        public Task UpdateAllLeagues(CancellationToken ct);
        public Task UpdateLeagueById(int id, CancellationToken ct);
    }
}