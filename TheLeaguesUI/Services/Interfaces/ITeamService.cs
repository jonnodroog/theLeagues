using Models.DTO;
using TheLeaguesUI.Extensions.Interfaces;

namespace TheLeaguesUI.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IOperationalResult<List<TeamDTO>>> GetAllTeams();
        Task<IOperationalResult<List<TeamDTO>>> GetAllTeamsInLeague(int leagueId);
        Task<IOperationalResult<TeamDTO>> GetTeamById(int teamId);
    }
}



