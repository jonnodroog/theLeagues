using TheLeaguesCMS.DAL;
using TheLeaguesCMS.Services.Interfaces;
using Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace TheLeaguesCMS.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly IDbContextFactory<TheLeaguesCMSDBContext> _contextFactory;

        public LeagueService(IDbContextFactory<TheLeaguesCMSDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddNewLeague(LeagueDTO league)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.AddAsync(league);
            await db.SaveChangesAsync();
        }

        public async Task DeleteLeague(int leagueId)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Leagues.Where(l => l.Id == leagueId).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<LeagueDTO>> GetAllLeaguesAsync()
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Leagues.ToListAsync();
        }

        public async Task<LeagueDTO?> GetLeagueByIdAsync(int id)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Leagues.FindAsync(id);
        }

        public async Task UpdateExistingLeague(LeagueDTO league)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            db.Leagues.Update(league);
            await db.SaveChangesAsync();
        }

        public async Task UpdateExistingLeagues(List<LeagueDTO> leagues)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            
            db.Leagues.UpdateRange(leagues);
            await db.SaveChangesAsync();
        }
    }
}