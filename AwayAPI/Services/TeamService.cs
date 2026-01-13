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
        public async Task UpdateAllTeams(CancellationToken ct)
        {
            try
            {
                //API-Football doesn't have a get all for teams
                //Need to get standings for each league of specified season
                //Team data can be found in standings for each team.
                // Transform data received into desired format.
                // Update database
                //TODO: Implement removal of teams from leagues if they no longer exist in standings.
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");


                foreach(var leagueId in _awaySettings.LeagueIdNumbers)
                {
                    var leagueInDatabase = await _context.Leagues.FindAsync(leagueId);
                    var standingsData = await client.GetFromJsonAsync<ApiResponse<LeagueDo>>($"standings?league={leagueId}&season={_awaySettings.CurrentSeason}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web),ct);

                    foreach(var standing in standingsData.Response[0].League.Standings)
                    {
                        var teamInDatabase = leagueInDatabase?.Teams.Find(t => t.Id == standing.Team.Id);

                        TeamDTO teamDTO = new()
                        {
                            Id = standing.Team.Id,
                            Name = standing.Team.Name,
                            Code = standing.Team.Code,
                            Country = standing.Team.Country,
                            Founded = standing.Team.Founded,
                            National = standing.Team.National,
                            Logo = standing.Team.Logo,
                            CurrentLeagueRank = standing.Rank,
                            Points = standing.Points,
                            GoalsDiff = standing.GoalsDiff,
                            Played = standing.All.Played,
                            Win = standing.All.Win,
                            Draw = standing.All.Draw,
                            Lose = standing.All.Lose,
                            GoalsFor = standing.All.Goals.For,
                            GoalsAgainst = standing.All.Goals.Against
                        };

                        if(leagueInDatabase.Teams.Exists(t => t.Id == teamDTO.Id))
                        {
                            teamInDatabase = teamDTO;
                        }
                        else
                        {
                            leagueInDatabase.Teams.Add(teamDTO);
                        }
                    }
                }
                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateTeamById(int id,CancellationToken ct)
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/teams?id=276
                // Call endpoint for speciified team ID.
                // Transform data received into desired format.
                // Update league value in database with latest versiion database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                var teamInDatabase = await _context.Teams.FindAsync(id);

                var teamDo = await client.GetFromJsonAsync<ApiResponse<TeamDo>>($"teams?id={id}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web),ct);
                var team = teamDo.Response[0].Team;
                var venue = teamDo.Response[0].Venue;
                
                _context.Teams.Entry(teamInDatabase).CurrentValues.SetValues(team);

                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                throw;
            }
        }
    }
}