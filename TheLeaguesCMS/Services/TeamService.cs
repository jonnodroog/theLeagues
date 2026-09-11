using TheLeaguesCMS.DAL;
using TheLeaguesCMS.Services.Interfaces;
using Models.DTO;
using Microsoft.EntityFrameworkCore;
using Models;

namespace TheLeaguesCMS.Services
{
    public class TeamService : ITeamService
    {
        private readonly IDbContextFactory<TheLeaguesCMSDBContext> _contextFactory;

        public TeamService(IDbContextFactory<TheLeaguesCMSDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddNewTeam(TeamDTO team)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Teams.AddAsync(team);
            await db.SaveChangesAsync();
        }

        public async Task AddNewTeams(List<TeamDTO> teams)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();

            foreach (var team in teams)
            {
                await db.Teams.AddAsync(team);
            }
            await db.SaveChangesAsync();
        }

        public async Task DeleteTeam(int teamId)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            await db.Teams.Where(t => t.Id == teamId).ExecuteDeleteAsync();
        }

        public async Task DeleteTeams(List<int> teamIds)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();

            foreach (var teamId in teamIds)
            {
                await db.Teams.Where(t => t.Id == teamId).ExecuteDeleteAsync();
            }
        }

        public async Task<IEnumerable<TeamDTO>> GetAllTeamsAsync()
        {
            await using var db = await _contextFactory.CreateDbContextAsync();
            return await db.Teams.ToListAsync();
        }

        public async Task<IEnumerable<TeamDTO>> GetTeamsByLeagueIdAsync(int leagueId)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();

            return await db.Teams
                .Include(t => t.League)
                .Where(t => t.League.Id == leagueId)
                .ToListAsync();
        }

        public async Task UpdateExistingTeam(TeamDTO team)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();

            db.Teams.Update(team);
            await db.SaveChangesAsync();
        }

        public async Task UpdateExistingTeams(List<TeamDTO> teams)
        {
            await using var db = await _contextFactory.CreateDbContextAsync();

            db.Teams.UpdateRange(teams);
            await db.SaveChangesAsync();
        }
    }
}