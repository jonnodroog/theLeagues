using System.Text.Json;
using Models.DTO;
using TheLeaguesUI.Extensions.Interfaces;
using TheLeaguesUI.Extensions;
using TheLeaguesUI.Services.Interfaces;

namespace TheLeaguesUI.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly HttpClient _httpClient;
        private List<LeagueDTO> _leagues = new();

        public LeagueService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IOperationalResult<List<LeagueDTO>>> GetAllLeagues()
        {
            //Operational pattern
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/leagues");
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return OperationResult<List<LeagueDTO>>.Failure("Could not fetch leagues for some reason.", (int)response.StatusCode);
                }

                using var responseStream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _leagues = await JsonSerializer.DeserializeAsync<List<LeagueDTO>>(responseStream, options) ?? new();

                return OperationResult<List<LeagueDTO>>.Success(_leagues ?? new());
            }
            catch (Exception ex)
            {
                return OperationResult<List<LeagueDTO>>.Failure(ex.Message, null);
            }
        }

        public async Task<IOperationalResult<LeagueDTO>> GetLeagueById(int leagueId)
        {
            //Operational pattern
            try
            {
                if (_leagues is not null && _leagues.Count > 0)
                {
                    var leagueSpecified = _leagues.FirstOrDefault(l => l.Id == leagueId);
                    return OperationResult<LeagueDTO>.Success(leagueSpecified ?? new());
                }
                else
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, $"/leagues/{leagueId}");
                    using var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return OperationResult<LeagueDTO>.Failure($"Could not fetch league with id {leagueId} for some reason.", (int)response.StatusCode);
                    }

                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var leagueFetched = await JsonSerializer.DeserializeAsync<LeagueDTO>(responseStream, options) ?? new();

                    return OperationResult<LeagueDTO>.Success(leagueFetched ?? new());
                }
            }
            catch (Exception ex)
            {
                return OperationResult<LeagueDTO>.Failure(ex.Message, null);
            }
        }
    }
}