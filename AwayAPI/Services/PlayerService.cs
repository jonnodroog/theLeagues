using AwayAPI.DAL;
using AwayAPI.Services.Interfaces;
using Extensions;
using Microsoft.Extensions.Options;
using Models;
using Models.DTO;

namespace AwayAPI.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly AwayDBContext _context;
        private readonly AwaySettings _awaySettings;
        private readonly IHttpClientFactory _httpClientFactory;
        public PlayerService(AwayDBContext context, IOptions<AwaySettings> awaySettings, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _awaySettings = awaySettings.Value;
            _httpClientFactory = httpClientFactory;
        }
        public async Task UpdateAllPlayers()
        {
            try
            {
                // Call Response | GET : https://v3.football.api-sports.io/players?id=39
                // Call endpoint for each league ID
                // Transform data received into desired format.
                // Update database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                foreach (var playerInDatabase in _context.Players)
                {
                    var playerResponse = await client.GetFromJsonAsync<ApiResponse<PlayerDo>>($"players/profiles?player={playerInDatabase.Id}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
                    var player = playerResponse.Response[0].Player;

                    _context.Players.Entry(playerInDatabase).CurrentValues.SetValues(player);
                }
                
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdatePlayer(int id)
        {
            try
            {
                //Call Response | GET : https://v3.football.api-sports.io/players/profiles?player=276
                // Call endpoint for speciified leage ID.
                // Transform data received into desired format.
                // Update league value in database with latest versiion database
                HttpClient client = _httpClientFactory.CreateClient(name: "apisports-football");

                var playerInDatabase = await _context.Players.FindAsync(id);
                var playerResponse = await client.GetFromJsonAsync<ApiResponse<PlayerDo>>($"players/profiles?player={id}", new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
                var player = playerResponse.Response[0].Player;

                _context.Players.Entry(playerInDatabase).CurrentValues.SetValues(player);

                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}