using Models.DTO;
using TheLeaguesUI.Extensions.Interfaces;

namespace TheLeaguesUI.Services.Interfaces
{
    public interface ILeagueService
    {
        //Usingg operational pattern here to return more than just a list of leagues, if it fails also return sfailure message
        Task<IOperationalResult<List<LeagueDTO>>> GetAllLeagues();
        Task<IOperationalResult<LeagueDTO>> GetLeagueById(int leagueId);
    }
}



