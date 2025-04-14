using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace HomeAPI.DAL
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<LeagueDTO> Leagues => Set<LeagueDTO>();
    }
}