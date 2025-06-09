using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTO;

public record TeamDTO
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name {get;set;} = "";
    public string Code {get;set;} = "";
    public string Country {get;set;} = "";
    public DateTime? Founded {get;set;}
    public bool National {get;set;}
    public string Logo {get;set;} = "";
    public int CurrentLeagueRank {get;set;}
    public int? Points {get;set;}
    public int? GoalsDiff {get;set;}
    public int? Played {get;set;}
    public int?  Win {get;set;}
    public int? Draw {get;set;}
    public int? Lose {get;set;}
    public int? GoalsFor {get;set;}
    public int? GoalsAgainst {get;set;}
    public List<PlayerDTO> Players {get;set;} = new();
    public LeagueDTO League {get;set;} = new();
}