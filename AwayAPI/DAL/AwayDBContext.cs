using System;
using Microsoft.EntityFrameworkCore;
using Models.DTO;

namespace AwayAPI.DAL;

public class AwayDBContext:DbContext
{
    public AwayDBContext(DbContextOptions options) : base(options){}
    public DbSet<LeagueDTO> Leagues {get;set;} = null!;
    public DbSet<TeamDTO> Teams {get;set;} = null!;
    public DbSet<PlayerDTO> Players {get;set;} = null!;

    

}
