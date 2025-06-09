using System.Runtime.CompilerServices;
using HomeAPI.DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTO;
using theLeagues.Services.Interfaces;

namespace HomeAPI.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly AppDbContext _context;

        public PlayerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlayerDTO>> GetAllPlayersAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<PlayerDTO> GetByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id) ?? new PlayerDTO();
        }

        public async Task<IEnumerable<PlayerDTO>> GetByTeamIdAsync(int teamId)
        {
            return await _context.Players.Where(p => p.Team != null && p.Team.Id == teamId).ToListAsync();
        }
        public async Task<IEnumerable<PlayerDTO>> GetByLeagueIdAsync(int leagueId)
        {
            return await _context.Players.Where(p => p.Team != null && p.Team.League.Id == leagueId).ToListAsync();
        }
        public async Task<IEnumerable<PlayerDTO>> GetByCountryAsync(string country)
        {
            return await _context.Players.Where(p => p.Nationality == country).ToListAsync();
        }
    }
}