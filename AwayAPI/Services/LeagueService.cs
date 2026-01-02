using AwayAPI.DAL;
using AwayAPI.Services.Interfaces;
using Extensions;
using Microsoft.Extensions.Options;
using Models;
using Models.DTO;

namespace AwayAPI.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly AwayDBContext _context;
        private readonly AwaySettings _awaySettings;
        private readonly IHttpClientFactory _httpClientFactory;
        public LeagueService(AwayDBContext context, IOptions<AwaySettings> awaySettings, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _awaySettings = awaySettings.Value;
            _httpClientFactory = httpClientFactory;
        }
        public async Task UpdateAllLeagues()
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/leagues?id=39
                // Call endpoint for each league ID
                // Transform data received into desired format.
                // Update database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                foreach (int leagueId in _awaySettings.LeagueIdNumbers)
                {
                    var leagueInDatabase = await _context.Leagues.FindAsync(leagueId);

                    var leagueDoParent = await client.GetFromJsonAsync<LeagueDoParent>($"leagues{leagueId}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
                    var leagueDoParentResponse = leagueDoParent?.Response;
                    var leagueDo = leagueDoParentResponse[0];
                    var league = leagueDo.league;

                    _context.Leagues.Entry(leagueInDatabase).CurrentValues.SetValues(league);

                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                throw;
            }
        }

        public Task UpdateLeague(int id)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch
            {
                throw;
            }
        }
    }
}