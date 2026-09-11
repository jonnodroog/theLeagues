using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace TheLeaguesCMS.DAL
{
    public class TheLeaguesCMSDBContext : DbContext
    {
        public TheLeaguesCMSDBContext(DbContextOptions options) : base(options){}
        public DbSet<LeagueDTO> Leagues => Set<LeagueDTO>();
        public DbSet<TeamDTO> Teams => Set<TeamDTO>();
        public DbSet<PlayerDTO> Players => Set<PlayerDTO>();
    }
}