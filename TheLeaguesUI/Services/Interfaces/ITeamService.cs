using Models.DTO;

namespace TheLeaguesUI.Services.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamDTO>> GetAllTeams();
        Task<List<TeamDTO>> GetAllTeamsInLeague(int leagueId);
        Task<TeamDTO> GetTeamById(int teamId);
    }
}



