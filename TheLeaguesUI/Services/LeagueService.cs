using Models.DTO;
using TheLeaguesUI.Services.Interfaces;

namespace TheLeaguesUI.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly HttpClient _httpClient;

        public LeagueService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<LeagueDTO>> GetAllLeaguesAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "/leagues");
                request.Headers.Add("Accept", "application/json");
            }catch
            {
                throw;
            }
        }

        public async Task<LeagueDTO> GetLeagueById(int leagueId)
        {
            throw new NotImplementedException();
        }
    }
}