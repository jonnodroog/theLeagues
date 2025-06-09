using HomeAPI.DAL;
using HomeAPI.Services.Interfaces;
using Models.DTO;

namespace HomeAPI.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly HomeDBContext _context;

        public LeagueService(HomeDBContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<LeagueDTO>> GetAllLeaguesAsync()
        {
            return Task.FromResult<IEnumerable<LeagueDTO>>(_context.Leagues);
        }

        public async Task<LeagueDTO> GetLeagueByIdAsync(int id)
        {
            return await _context.Leagues.FindAsync(id) ?? new LeagueDTO();
        }
    }
}