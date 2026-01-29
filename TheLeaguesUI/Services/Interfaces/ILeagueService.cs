using Models.DTO;

namespace TheLeaguesUI.Services.Interfaces
{
    public interface ILeagueService
    {
        Task<List<LeagueDTO>> GetAllLeagues();
        Task<LeagueDTO> GetLeagueById(int leagueId);
    }
}



