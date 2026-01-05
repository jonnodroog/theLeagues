using AwayAPI.DAL;
using AwayAPI.Services.Interfaces;
using Extensions;
using Microsoft.Extensions.Options;
using Models;
using Models.DTO;

namespace AwayAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly AwayDBContext _context;
        private readonly AwaySettings _awaySettings;
        private readonly IHttpClientFactory _httpClientFactory;
        public TeamService(AwayDBContext context, IOptions<AwaySettings> awaySettings, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _awaySettings = awaySettings.Value;
            _httpClientFactory = httpClientFactory;
        }
        public async Task UpdateAllTeams()
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/teams?id=39
                // Call endpoint for each team ID
                // Transform data received into desired format.
                // Update database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                foreach (var teamInDatabase in _context.Teams)
                {
                    var teamDo = await client.GetFromJsonAsync<ApiResponse<TeamDo>>($"teams?id={teamInDatabase.Id}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
                    var team = teamDo.Response[0].Team;
                    var venue = teamDo.Response[0].Venue;

                    _context.Teams.Entry(teamInDatabase).CurrentValues.SetValues(team);
                }

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateTeam(int id)
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/teams?id=276
                // Call endpoint for speciified team ID.
                // Transform data received into desired format.
                // Update league value in database with latest versiion database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                var teamInDatabase = await _context.Teams.FindAsync(id);

                var teamDo = await client.GetFromJsonAsync<ApiResponse<TeamDo>>($"teams?id={id}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
                var team = teamDo.Response[0].Team;
                var venue = teamDo.Response[0].Venue;
                
                _context.Teams.Entry(teamInDatabase).CurrentValues.SetValues(team);

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}