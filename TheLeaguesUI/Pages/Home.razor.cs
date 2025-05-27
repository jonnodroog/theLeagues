using Microsoft.AspNetCore.Components;
using Models;
using Models.DTO;

namespace TheLeaguesUI.Pages
{
    public partial class Home : ComponentBase
    {
        public List<LeagueDTO> leagues = new();
    }
}