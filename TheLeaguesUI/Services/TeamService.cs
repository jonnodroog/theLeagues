using Models.DTO;
using TheLeaguesUI.Services.Interfaces;

namespace TheLeaguesUI.Services
{
    public class TeamService : ITeamService
    {
        public TeamService()
        {
            
        }
        public Task<List<TeamDTO>> GetAllTeams()
        {
            throw new NotImplementedException();
        }

        public Task<List<TeamDTO>> GetAllTeamsInLeague(int leagueId)
        {
            throw new NotImplementedException();
        }

        public Task<TeamDTO> GetTeamById(int teamId)
        {
            throw new NotImplementedException();
        }
    }
}