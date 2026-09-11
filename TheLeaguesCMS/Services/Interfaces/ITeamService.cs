using Models.DTO;

namespace TheLeaguesCMS.Services.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDTO>> GetAllTeamsAsync();
        Task<IEnumerable<TeamDTO>> GetTeamsByLeagueIdAsync(int leagueId);
        Task AddNewTeam(TeamDTO team);
        Task AddNewTeams(List<TeamDTO> teams);
        Task UpdateExistingTeam(TeamDTO team);
        Task UpdateExistingTeams(List<TeamDTO> teams);
        Task DeleteTeam(int teamId);
        Task DeleteTeams(List<int> teamIds);
    }
}