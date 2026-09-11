using TheLeaguesCMS.DAL;
using TheLeaguesCMS.Services.Interfaces;
using Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Models;

namespace TheLeaguesCMS.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IDbContextFactory<TheLeaguesCMSDBContext> _contextFactory;

        public PlayerService(IDbContextFactory<TheLeaguesCMSDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddNewPlayer(PlayerDTO player)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Players.AddAsync(player);
            await db.SaveChangesAsync();
        }

        public async Task AddNewPlayers(List<PlayerDTO> players)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            foreach(var player in players)
            {
                await db.Players.AddAsync(player);
            }

            await db.SaveChangesAsync();
        }

        public async Task DeletePlayer(int playerId)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Players.Where(p => p.Id == playerId).ExecuteDeleteAsync();
        }

        public async Task DeletePlayers(List<int> playerIds)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            foreach(var playerId in playerIds)
            {
                await db.Players.Where(p => p.Id == playerId).ExecuteDeleteAsync();
            }
        }

        public async Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync()
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Players.ToListAsync();
        }

        public async Task<PlayerDTO?> GetPlayerByPlayerIdAsync(int playerId)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Players.FindAsync(playerId);
        }

        public async Task<IEnumerable<PlayerDTO>> GetPlayersByLeagueIdAsync(int leagueId)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Players
                .Include(p => p.Team)
                    .ThenInclude(t => t!.League)
                .Where(p => p.Team != null && p.Team.League.Id == leagueId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerDTO>> GetPlayersByTeamIdAsync(int teamId)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Players
                .Include(p => p.Team)
                .Where(p => p.Team != null && p.Team.Id == teamId)
                .ToListAsync();
        }

        public async Task UpdateExistingPlayer(PlayerDTO player)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            db.Players.Update(player);
            await db.SaveChangesAsync();
        }

        public async Task UpdateExistingPlayers(List<PlayerDTO> players)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();

            db.Players.UpdateRange(players);
            await db.SaveChangesAsync();
        }
    }
}