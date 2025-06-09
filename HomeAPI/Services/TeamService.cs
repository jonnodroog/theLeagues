using HomeAPI.DAL;
using HomeAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using theLeagues.Services.Interfaces;

namespace HomeAPI.Services
{
    public class TeamService : ITeamService
    {
        private readonly HomeDBContext _context;

        public TeamService(HomeDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeamDTO>> GetAllTeamsAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<TeamDTO> GetByIdAsync(int id)
        {
            return await _context.FindAsync<TeamDTO>(id) ?? new TeamDTO();
        }

        public async Task<IEnumerable<TeamDTO>> GetByLeagueIdAsync(int leagueId)
        {
            return await _context.Teams.Where(t => t.League.Id == leagueId).ToListAsync();
        }

        public async Task<IEnumerable<TeamDTO>> GetByCountryAsync(string country)
        {
            return await _context.Teams.Where(t => t.Country == country).ToListAsync();
        }
    }
}