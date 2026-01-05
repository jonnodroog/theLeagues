namespace AwayAPI.Services.Interfaces
{
    public interface ITeamService
    {
        public Task UpdateAllTeams(CancellationToken ct);
        public Task UpdateTeamById(int id,CancellationToken ct);
    }
}