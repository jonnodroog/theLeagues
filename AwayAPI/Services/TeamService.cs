using AwayAPI.DAL;
using AwayAPI.Services.Interfaces;
using Extensions;
using Microsoft.EntityFrameworkCore;
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


                foreach (var leagueId in _awaySettings.LeagueIdNumbers)
                {
                    var leagueInDatabase = await _context.Leagues.Include(l => l.Teams).FirstOrDefaultAsync(l => l.Id == leagueId, ct);
                    var standingsData = await client.GetFromJsonAsync<ApiResponse<LeagueStandingDo>>($"standings?league={leagueId}&season={_awaySettings.CurrentSeason}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web), ct);
                    var leagueFromStandingsCall = standingsData.Response.First().League;

                    if (leagueInDatabase is not null)
                    {
                        foreach (var standing in standingsData.Response.First().League.Standings.First())
                        {
                            var teamInDatabase = leagueInDatabase?.Teams.FirstOrDefault(t => t.Id == standing.Team.Id);

                            TeamDTO teamDTO = new()
                            {
                                Id = standing.Team.Id,
                                Name = standing.Team.Name,
                                Code = standing.Team.Code,
                                Country = leagueFromStandingsCall.Country,
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

                            if (teamInDatabase is not null)
                            {
                                _context.Teams.Entry(teamInDatabase).CurrentValues.SetValues(teamDTO);
                            }
                            else
                            {
                                leagueInDatabase?.Teams.Add(teamDTO);
                            }
                        }
                        Console.WriteLine($"\n\nALL TEAMS HAVE BEEN ADDED FOR {standingsData.Response.First().League.Name}\n\n");
                    }
                }

                await _context.SaveChangesAsync(ct);
            }
            catch
            {
                throw;
            }
            finally
            {
                Console.WriteLine("\n\nALL TEAMS HAVE BEEN SUCCESSFULLY DDED\n\n");
            }
        }

        public async Task UpdateTeamById(int id, CancellationToken ct)
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/teams?id=276
                // Call endpoint for speciified team ID.
                // Transform data received into desired format.
                // Update league value in database with latest versiion database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                var teamInDatabase = await _context.Teams.FindAsync(id);

                if (teamInDatabase is not null)
                {
                    var teamDo = await client.GetFromJsonAsync<ApiResponse<TeamDo>>($"teams?id={id}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web), ct);
                    var team = teamDo.Response[0].Team;
                    var venue = teamDo.Response[0].Venue;

                    TeamDTO teamDTO = new()
                    {
                        Id = team.Id,
                        Name = team.Name,
                        Code = team.Code,
                        Country = team.Country,
                        Founded = team.Founded,
                        National = team.National,
                        Logo = team.Logo
                    };

                    _context.Teams.Entry(teamInDatabase).CurrentValues.SetValues(team);

                    await _context.SaveChangesAsync(ct);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}