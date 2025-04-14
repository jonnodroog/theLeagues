using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace HomeAPI.DAL
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<LeagueDTO> Leagues => Set<LeagueDTO>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Optional: Configure read-only SQLite (if you open the DB manually in read-only mode)
        }
    }
}