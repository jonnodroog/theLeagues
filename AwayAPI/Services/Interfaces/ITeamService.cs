namespace AwayAPI.Services.Interfaces
{
    public interface ITeamService
    {
        public Task UpdateAllTeams();
        public Task UpdateTeam(int id);
    }
}