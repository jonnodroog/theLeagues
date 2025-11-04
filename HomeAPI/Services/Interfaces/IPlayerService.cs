using Models.DTO;

namespace theLeagues.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync();
        Task<PlayerDTO> GetPlayerByPlayerIdAsync(int id);
        Task<IEnumerable<PlayerDTO>> GetPlayersByTeamIdAsync(int teamId);
        Task<IEnumerable<PlayerDTO>> GetPlayersByLeagueIdAsync(int leagueId);
    }
}