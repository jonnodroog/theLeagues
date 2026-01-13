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
        public async Task UpdateAllLeagues(CancellationToken ct)
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/leagues/
                // Call endpoint for all leagues
                // Transform data received into desired format i.e. LeagueDTO object.
                // Update database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                foreach (int leagueId in _awaySettings.LeagueIdNumbers)
                {
                    var leagueInDatabase = await _context.Leagues.FindAsync(leagueId);

                    var leagueDo = await client.GetFromJsonAsync<ApiResponse<LeagueDo>>($"leagues?id={leagueId}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web), ct);

                    var league = leagueDo?.Response[0].League;
                    var country = leagueDo?.Response[0].Country;

                    LeagueDTO leagueDTO = new()
                    {
                        Id = league.Id,
                        Name = league.Name,
                        Country = country.Name,
                        Logo = league.Logo,
                        Flag = country.Flag,
                    };

                    if (leagueInDatabase is not null)
                    {
                        _context.Leagues.Entry(leagueInDatabase).CurrentValues.SetValues(leagueDTO);
                    }
                    else
                    {
                        _context.Leagues.Add(leagueDTO);
                    }
                }

                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateLeagueById(int id, CancellationToken ct)
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/leagues?id=39
                // Call endpoint for speciified leage ID.
                // Transform data received into desired format i.e. LeagueDTO object
                // Update league value in database with latest versiion database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                var leagueInDatabase = await _context.Leagues.FindAsync(id);
                var leagueDo = await client.GetFromJsonAsync<ApiResponse<LeagueDo>>($"leagues?id={id}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web), ct);

                var league = leagueDo?.Response[0].League;
                var country = leagueDo?.Response[0].Country;

                LeagueDTO leagueDTO = new()
                {
                    Id = league.Id,
                    Name = league.Name,
                    Country = country.Name,
                    Logo = league.Logo,
                    Flag = country.Flag,
                };

                if (leagueInDatabase is not null)
                {
                    _context.Leagues.Entry(leagueInDatabase).CurrentValues.SetValues(leagueDTO);
                }
                else
                {
                    _context.Leagues.Add(leagueDTO);
                }
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                throw;
            }
        }
    }
}