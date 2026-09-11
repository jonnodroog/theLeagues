using Models.DTO;

namespace TheLeaguesCMS.Services.Interfaces
{
    public interface ILeagueService
    {
        Task<IEnumerable<LeagueDTO>> GetAllLeaguesAsync();
        Task<LeagueDTO?> GetLeagueByIdAsync(int id);
        Task AddNewLeague(LeagueDTO league);
        Task UpdateExistingLeague(LeagueDTO league);
        Task DeleteLeague(int leagueId);
    }
}