using System.Text.Json;
using AwayAPI.DAL;
using Extensions;
using Microsoft.Extensions.Options;
using Models;
using Models.DTO;

public class AwayBackgroundService : BackgroundService, IDisposable
{
    private readonly ILogger<AwayBackgroundService> _logger;
    private readonly AwaySettings _awaySettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AwayBackgroundService(ILogger<AwayBackgroundService> logger, IOptions<AwaySettings> awaySettings, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _awaySettings = awaySettings.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _logger.LogInformation("AwayBackgroundService has started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using(var scope = _serviceScopeFactory.CreateAsyncScope())
            {
                var _awayDbContext = scope.ServiceProvider.GetRequiredService<AwayDBContext>();
            // Fetch all leagues data and complete leaguesDTOs
            _logger.LogInformation("Fetching all leagues data. Teams and player raw date included.");
            List<League> leagues = await GetAllLeagues();
            _logger.LogInformation("All leagues data has been fetched!");

            _logger.LogInformation("Converting leagues to a nicer format for the UI. Teams and player raw date included.");
            List<LeagueDTO> leaguesButNicer = await CompleteLeaguesDTO(leagues);
            _logger.LogInformation("Leagues are now in a nicer format.");

            //store in database
            foreach(var league in leaguesButNicer)
            {
                await _awayDbContext.Leagues.AddAsync(league);
                
                foreach(var team in league.Teams)
                {
                    await _awayDbContext.Teams.AddAsync(team);
                    foreach(var player in team.Players)
                    {
                        await _awayDbContext.Players.AddAsync(player);
                    }
                }
            }

            await _awayDbContext.SaveChangesAsync();
            }

        }
    }

    private async Task<List<League>> GetAllLeagues()
    {
        try
        {
            await Task.Run(() => _logger.LogDebug("Starting GetAllLeagues method"));
            // Initialize leagues list that will get put into database
            List<League> leagues = new();

            //Initialize http client that will be used to send and receive API requests.   
            var client = new HttpClient();

            await Task.Run(() => _logger.LogDebug("Stepping through each league ID."));

            // Step through pre-deterimined list of leagues.
            foreach (int leagueIdNumber in _awaySettings.LeagueIdNumbers)
            {
                await Task.Run(() => _logger.LogDebug("Fetching data for league id = " + leagueIdNumber));

                // Setup request message
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/standings?league={leagueIdNumber}&season=2024"),
                    Headers =
                            {
                                { "x-rapidapi-key", _awaySettings.API_Key },
                                { "x-rapidapi-host", _awaySettings.API_Host }
                            }
                };

                await Task.Run(() => _logger.LogDebug($"URL used for league id = {leagueIdNumber}: https://api-football-v1.p.rapidapi.com/v3/standings?league={leagueIdNumber}&season=2024"));

                // Send and log response.
                using (var response = await client.SendAsync(request))
                {
                    await Task.Run(() => _logger.LogDebug("Sending request."));
                    var body = await response.Content.ReadAsStringAsync();


                    LeagueDoParent leagueDoParent = JsonSerializer.Deserialize<LeagueDoParent>(body, new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});
                    Console.Write(leagueDoParent.Response[0].league);
                    leagues.Add(leagueDoParent.Response[0].league);
                }

                Thread.Sleep(5000);
            }
            return leagues;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to fetch all the leagues. Error: " + ex.Message);
            throw;
        }
    }

    private async Task<List<LeagueDTO>> CompleteLeaguesDTO(List<League> leagues)
    {
        try
        {
            _logger.LogDebug("Starting CompleteLeaguesDTO conversion");

            List<LeagueDTO> niceLeagues = new();

            foreach(var league in leagues)
            {
                LeagueDTO niceLeague = new()
                {
                    Id = league.Id,
                    Name = league.Name,
                    Country = league.Country,
                    Logo = league.Logo,
                    Flag = league.Flag,
                    Season = league.Season,
                    Teams = await CompleteTeamsDTO(league.Standings[0])
                };

                niceLeagues.Add(niceLeague);
            }

            return niceLeagues;
        }catch
        {
            throw;
        }
    }

    private async Task<List<TeamDTO>> CompleteTeamsDTO(List<Standing> standings)
    {
        try
        {
            _logger.LogDebug("Starting TeamsDTO conversion");
            List<TeamDTO> niceTeams = new();

            foreach(Standing standing in standings)
            {
                TeamDTO team = new TeamDTO()
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
                    GoalsAgainst = standing.All.Goals.Against,
                    GoalsFor = standing.All.Goals.For,
                    Players = await GetAllPlayersByTeamId(standing.Team.Id)

                };

                niceTeams.Add(team);
            }

            return niceTeams;
            
        }catch
        {
            throw;
        }
    }

    private async Task<List<PlayerDTO>> GetAllPlayersByTeamId(int teamId)
    {
        try
        {
             await Task.Run(() => _logger.LogDebug("Starting GetAllPlayersByTeamId method"));
            // Initialize okayers list that will get put into database
            List<Player> players = new();

            //Initialize http client that will be used to send and receive API requests.   
            var client = new HttpClient();

                await Task.Run(() => _logger.LogDebug("Fetching player data for team id = " + teamId));

            // Setup request messag
            var request = new HttpRequestMessage
            {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/players/squads?team={teamId}"),
                    Headers =
                            {
                                { "x-rapidapi-key", _awaySettings.API_Key },
                                { "x-rapidapi-host", _awaySettings.API_Host }
                            }
                };

                await Task.Run(() => _logger.LogDebug($"URL used for team id = {teamId}: https://api-football-v1.p.rapidapi.com/v3/players/squads?team={teamId}"));

                // Send and log response.
                using (var response = await client.SendAsync(request))
                {
                    await Task.Run(() => _logger.LogDebug("Sending request."));
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(body);


                    PlayerDoParent playerDoParent = JsonSerializer.Deserialize<PlayerDoParent>(body, new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});
                    players = playerDoParent.Response[0].Players;
                }


                Thread.Sleep(5000);
                return await CompletePlayersDTO(players);
        }
        catch
        {
            throw;
        }
    }

    private async Task<List<PlayerDTO>> CompletePlayersDTO(List<Player> players)
    {
        try
        {
            List<PlayerDTO> nicePlayers = new();

            foreach(var player in players)
            {
                PlayerDTO nicePlayer = new()
                {
                    Id = player.Id,
                    Name = player.Name,
                    Age = player.Age,
                    Number = player.Number,
                    Position = player.Position,
                    Photo = player.Photo
                };

                nicePlayers.Add(nicePlayer);
            }

            return nicePlayers;
        }catch
        {
            throw;
        }
    }
}