using Models.DTO;

namespace HomeAPI.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDTO>> GetAllTeamsAsync();
        Task<TeamDTO> GetByIdAsync(int id);
        Task<IEnumerable<TeamDTO>> GetByLeagueIdAsync(int leagueId);
        Task<IEnumerable<TeamDTO>> GetByCountryAsync(string country);
    }
}