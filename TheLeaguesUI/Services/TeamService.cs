using System.Text.Json;
using Models.DTO;
using TheLeaguesUI.Extensions.Interfaces;
using TheLeaguesUI.Extensions;
using TheLeaguesUI.Services.Interfaces;

namespace TheLeaguesUI.Services
{
    public class TeamService : ITeamService
    {
        private readonly HttpClient _httpClient;
        private List<TeamDTO> _teams = new();

        public TeamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IOperationalResult<List<TeamDTO>>> GetAllTeams()
        {
            //Operational pattern
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/teams");
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return OperationResult<List<TeamDTO>>.Failure("Could not fetch all teams for some reason.", (int)response.StatusCode);
                }

                using var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _teams = await JsonSerializer.DeserializeAsync<List<TeamDTO>>(responseStream, options) ?? new();

                return OperationResult<List<TeamDTO>>.Success(_teams ?? new());
            }
            catch (Exception ex)
            {
                return OperationResult<List<TeamDTO>>.Failure(ex.Message, null);
            }
        }

        public async Task<IOperationalResult<TeamDTO>> GetTeamById(int teamId)
        {
            //Operational pattern
            try
            {
                if (_teams is not null && _teams.Count > 0)
                {
                    var teamSpecified = _teams.FirstOrDefault(t => t.Id == teamId);
                    return OperationResult<TeamDTO>.Success(teamSpecified ?? new());
                }
                else
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"/teams/{teamId}");
                    using var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return OperationResult<TeamDTO>.Failure($"Could not fetch team with id {teamId} for some reason.", (int)response.StatusCode);
                    }

                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var teamFetched = await JsonSerializer.DeserializeAsync<TeamDTO>(responseStream, options) ?? new();

                    return OperationResult<TeamDTO>.Success(teamFetched ?? new());
                }
            }
            catch (Exception ex)
            {
                return OperationResult<TeamDTO>.Failure(ex.Message, null);
            }
        }

        public async Task<IOperationalResult<List<TeamDTO>>> GetAllTeamsInLeague(int leagueId)
        {
            //Operational pattern
            try
            {
                if (_teams is not null && _teams.Count > 0)
                {
                    var allTeamsInLeagueSpecified = _teams.Where(t => t.League != null && t.League.Id == leagueId).ToList();
                    return OperationResult<List<TeamDTO>>.Success(allTeamsInLeagueSpecified ?? new());
                }
                else
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"/leagues/{leagueId}");
                    using var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        using var responseStream = await response.Content.ReadAsStreamAsync();
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var leagueFetched = await JsonSerializer.DeserializeAsync<LeagueDTO>(responseStream, options) ?? new();

                        if (leagueFetched is not null && leagueFetched.Teams.Count > 0)
                        {
                            return OperationResult<List<TeamDTO>>.Success(leagueFetched.Teams ?? new());

                        }
                    }
                    return OperationResult<List<TeamDTO>>.Failure($"Could not fetch teams in league with league id {leagueId} for some reason.", (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return OperationResult<List<TeamDTO>>.Failure(ex.Message, null);
            }
        }
    }
}