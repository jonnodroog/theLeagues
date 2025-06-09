using Models.DTO;

namespace theLeagues.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync();
        Task<PlayerDTO> GetByIdAsync(int id);
        Task<IEnumerable<PlayerDTO>> GetByTeamIdAsync(int teamId);
        Task<IEnumerable<PlayerDTO>> GetByLeagueIdAsync(int leagueId);
        Task<IEnumerable<PlayerDTO>> GetByCountryAsync(string country);
    }
}