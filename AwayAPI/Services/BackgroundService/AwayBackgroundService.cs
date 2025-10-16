using System.Text.Json;
using AwayAPI.DAL;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
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
        using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var _awayDbContext = scope.ServiceProvider.GetRequiredService<AwayDBContext>();
            // Fetch all leagues data and complete leaguesDTOs
            _logger.LogInformation("Fetching all leagues data. Teams and player raw date included.");
            List<League> leagues = new();
            List<LeagueDTO> leaguesDummyData = new();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                _logger.LogInformation("This is a development environment. Fetching dummy data.");
                leaguesDummyData = await GetAllLeagesDummyData();
            }
            else
            {
                _logger.LogInformation("This is a production or staging environment. Fetching data.");
                leagues = await GetAllLeagues();
            }
            _logger.LogInformation("All leagues data has been fetched!");

            _logger.LogInformation("Converting leagues to a nicer format for the UI. Teams and player raw data included.");
            List<LeagueDTO> leaguesButNicer = new();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                leaguesButNicer = leaguesDummyData;
            } else
            {
                leaguesButNicer = await CompleteLeaguesDTO(leagues);
            }
            _logger.LogInformation("Leagues are now in a nicer format.");

            //Update, delete or Add the new league data.
            _logger.LogInformation("Updating/Adding league data to database.");
            foreach (var league in leaguesButNicer)
            {
                var leagueFromDb = await _awayDbContext.Leagues.FindAsync(league.Id);

                if (leagueFromDb != null)
                {
                    leagueFromDb.Id = league.Id;
                    leagueFromDb.Country = league.Country;
                    leagueFromDb.Flag = league.Country;
                    leagueFromDb.Logo = league.Logo;
                    leagueFromDb.Name = league.Name;
                    leagueFromDb.Season = league.Season;
                }
                else
                {
                    await _awayDbContext.Leagues.AddAsync(league);
                }

                foreach (var team in league.Teams)
                {
                    var teamFromDb = await _awayDbContext.Teams.FindAsync(team.Id);

                    if (teamFromDb != null)
                    {
                        teamFromDb.Id = team.Id;
                        teamFromDb.Code = team.Code;
                        teamFromDb.Country = team.Country;
                        teamFromDb.CurrentLeagueRank = team.CurrentLeagueRank;
                        teamFromDb.Draw = team.Draw;
                        teamFromDb.Founded = team.Founded;
                        teamFromDb.GoalsAgainst = team.GoalsAgainst;
                        teamFromDb.GoalsFor = team.GoalsFor;
                        teamFromDb.GoalsDiff = team.GoalsDiff;
                        teamFromDb.Logo = team.Logo;
                        teamFromDb.Lose = team.Lose;
                        teamFromDb.Name = team.Name;
                        teamFromDb.Points = team.Points;
                        teamFromDb.National = team.National;
                        teamFromDb.Played = team.Played;
                        teamFromDb.Win = team.Win;
                    }
                    else
                    {
                        await _awayDbContext.Teams.AddAsync(team);
                    }

                    foreach (var player in team.Players)
                    {
                        var playerFromDb = await _awayDbContext.Players.FindAsync(player.Id);

                        if (playerFromDb != null)
                        {
                            playerFromDb.Age = player.Age;
                            playerFromDb.FirstName = player.FirstName;
                            playerFromDb.Height = player.Height;
                            playerFromDb.Id = player.Id;
                            playerFromDb.LastName = player.LastName;
                            playerFromDb.Name = player.Name;
                            playerFromDb.Nationality = player.Nationality;
                            playerFromDb.Number = player.Number;
                            playerFromDb.Photo = player.Photo;
                            playerFromDb.Position = player.Position;
                        }
                        else
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
                    RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/standings?league={leagueIdNumber}&season={_awaySettings.CurrentSeason ?? 2025}"),
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

    private async Task<List<LeagueDTO>> GetAllLeagesDummyData()
    {
        try
        {
            List<LeagueDTO> leagues = new();


            var leaguesDummyDataRoot = JsonDocument.Parse(await File.ReadAllTextAsync("D:\\Work\\Projects\\theLeagues\\DATA\\Leagues.json"));
            var TeamsDummyData = JsonDocument.Parse(await File.ReadAllTextAsync("D:\\Work\\Projects\\theLeagues\\DATA\\Teams.json"));
            var PlayersDummyData = JsonDocument.Parse(await File.ReadAllTextAsync("D:\\Work\\Projects\\theLeagues\\DATA\\Players.json"));

            foreach (var league in leaguesDummyDataRoot.RootElement.EnumerateArray())
            {
                LeagueDTO dummyLeague = new();
                dummyLeague.Id = league.TryGetProperty("Id", out var leagueId) ? leagueId.GetInt32() : -1;
                dummyLeague.Name = league.TryGetProperty("Name", out var leagueName) ? leagueName.GetString() : "";
                dummyLeague.Country = league.TryGetProperty("Country", out var leagueCountry) ? leagueCountry.GetString() : "";
                dummyLeague.Logo = league.TryGetProperty("Logo", out var leagueLogo) ? leagueLogo.GetString() : "";
                dummyLeague.Flag = league.TryGetProperty("Flag", out var leagueFlag) ? leagueFlag.GetString() : "";
                dummyLeague.Season = league.TryGetProperty("Season", out var leagueSeason) ? leagueSeason.GetInt32() : -1;

                foreach (var team in TeamsDummyData.RootElement.EnumerateArray())
                {
                    if ((team.TryGetProperty("LeagueId", out leagueId) ? leagueId.GetInt32() : -1) == dummyLeague.Id)
                    {
                        TeamDTO dummyTeam = new();
                        dummyTeam.Id = team.TryGetProperty("Id", out var teamId) ? teamId.GetInt32() : -1;
                        dummyTeam.Name = team.TryGetProperty("Name", out var teamName) ? teamName.GetString() : "";
                        dummyTeam.Code = team.TryGetProperty("Code", out var teamCode) ? teamCode.GetString() : "";
                        dummyTeam.Country = team.TryGetProperty("Country", out var teamCountry) ? teamCountry.GetString() : "";
                        dummyTeam.Logo = team.TryGetProperty("Logo", out var teamLogo) ? teamLogo.GetString() : "";
                        dummyTeam.CurrentLeagueRank = team.TryGetProperty("CurrentLeagueRank", out var teamCurrentLeaguueRank) ? teamCurrentLeaguueRank.GetInt32() : -1;
                        dummyTeam.Points = team.TryGetProperty("Points", out var teamPoints) ? teamPoints.GetInt32() : -1;
                        dummyTeam.GoalsDiff = team.TryGetProperty("GoalsDiff", out var teamGoalsDiff) ? teamGoalsDiff.GetInt32() : -1;
                        dummyTeam.Played = team.TryGetProperty("Played", out var teamPlayed) ? teamPlayed.GetInt32() : -1;
                        dummyTeam.Win = team.TryGetProperty("Win", out var teamWin) ? teamWin.GetInt32() : -1;
                        dummyTeam.Draw = team.TryGetProperty("Draw", out var teamDraw) ? teamDraw.GetInt32() : -1;
                        dummyTeam.Lose = team.TryGetProperty("Lose", out var teamLose) ? teamLose.GetInt32() : -1;
                        dummyTeam.GoalsAgainst = team.TryGetProperty("GoalsAgainst", out var teamGoalsAgainst) ? teamGoalsAgainst.GetInt32() : -1;
                        dummyTeam.GoalsFor = team.TryGetProperty("GoalsFor", out var teamGoalsFor) ? teamGoalsFor.GetInt32() : -1;

                        foreach (var player in PlayersDummyData.RootElement.EnumerateArray())
                        {
                            if ((player.TryGetProperty("TeamId", out var _teamId) ? _teamId.GetInt32() : -1) == dummyTeam.Id)
                            {
                                PlayerDTO dummyPlayer = new();
                                dummyPlayer.Id = player.TryGetProperty("Id", out var playerId) ? playerId.GetInt32() : -1;
                                dummyPlayer.Name = player.TryGetProperty("Name", out var playerName) ? playerName.GetString() : "";
                                dummyPlayer.Age = player.TryGetProperty("Age", out var playerAge) ? playerAge.GetInt32() : -1;
                                dummyPlayer.Number = player.TryGetProperty("Number", out var playerNumber) ? playerNumber.GetInt32() : -1;
                                dummyPlayer.Position = player.TryGetProperty("Position", out var playerPosition) ? playerPosition.GetString() : "";
                                dummyPlayer.Photo = player.TryGetProperty("Photo", out var playerPhoto) ? playerPhoto.GetString() : "";

                                dummyTeam.Players.Add(dummyPlayer);
                            }
                        }

                        dummyLeague.Teams.Add(dummyTeam);
                    }
                }
                
                leagues.Add(dummyLeague);
            }

            return leagues;
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to fetch dunmmy data for leagues. \nError: {ex.Message}. ");
        }
    }
    private async Task<List<LeagueDTO>> CompleteLeaguesDTO(List<League> leagues)
    {
        try
        {
            _logger.LogDebug("Starting CompleteLeaguesDTO conversion");

            List<LeagueDTO> niceLeagues = new();

            foreach (var league in leagues)
            {
                LeagueDTO niceLeague = new()
                {
                    Id = league.Id,
                    Name = league.Name,
                    Country = league.Country,
                    Logo = league.Logo,
                    Flag = league.Flag,
                    Season = league.Season,
                    Teams = await CompleteTeamsDTO(league)
                };

                niceLeagues.Add(niceLeague);
            }

            return niceLeagues;
        }
        catch
        {
            throw;
        }
    }

    private async Task<List<TeamDTO>> CompleteTeamsDTO(League league)
    {
        try
        {
            _logger.LogDebug("Starting TeamsDTO conversion");
            List<TeamDTO> niceTeams = new();

            foreach(Standing standing in league.Standings[0])
            {
                TeamDTO team = new TeamDTO()
                {
                    Id = standing.Team.Id,
                    Name = standing.Team.Name,
                    Code = standing.Team.Code,
                    Country = league.Country,
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