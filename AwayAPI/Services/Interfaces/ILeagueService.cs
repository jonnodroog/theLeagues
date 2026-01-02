namespace AwayAPI.Services.Interfaces
{
    public interface ILeagueService
    {
        public Task UpdateAllLeagues();
        public Task UpdateLeague(int id);
    }
}