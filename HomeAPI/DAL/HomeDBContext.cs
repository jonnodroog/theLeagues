using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace HomeAPI.DAL
{
    public class HomeDBContext(DbContextOptions<HomeDBContext> options) : DbContext(options)
    {
        public DbSet<LeagueDTO> Leagues => Set<LeagueDTO>();
        public DbSet<TeamDTO> Teams => Set<TeamDTO>();
        public DbSet<PlayerDTO> Players => Set<PlayerDTO>();
    }
}