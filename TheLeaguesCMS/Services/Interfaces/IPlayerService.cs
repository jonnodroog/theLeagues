using Models.DTO;

namespace TheLeaguesCMS.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync();
        Task<PlayerDTO?> GetPlayerByPlayerIdAsync(int id);
        Task<IEnumerable<PlayerDTO>> GetPlayersByTeamIdAsync(int teamId);
        Task<IEnumerable<PlayerDTO>> GetPlayersByLeagueIdAsync(int leagueId);
        Task AddNewPlayer(PlayerDTO player);
        Task AddNewPlayers(List<PlayerDTO> players);
        Task UpdateExistingPlayer(PlayerDTO player);
        Task UpdateExistingPlayers(List<PlayerDTO> players);
        Task DeletePlayer(int playerId);
        Task DeletePlayers(List<int> playerIds);
    }
}