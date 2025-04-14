using Microsoft.AspNetCore.Components;
using Models;
using Models.DTO;

namespace TheLeaguesUI.Pages
{
    public partial class Home : ComponentBase
    {
        public List<LeagueDTO> leagues = new();

        private async Task<List<LeagueDTO>> GetAllLeagues()
        {
            try
            {
                return new List<LeagueDTO>();
            }catch(Exception ex)
            {
                throw;
            }
        }
    }
}