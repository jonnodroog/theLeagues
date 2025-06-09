using Models.DTO;

namespace HomeAPI.Services.Interfaces
{
    public interface ILeagueService
    {
        Task<IEnumerable<LeagueDTO>> GetAllLeaguesAsync();
        Task<LeagueDTO> GetLeagueByIdAsync(int id);
    }
}