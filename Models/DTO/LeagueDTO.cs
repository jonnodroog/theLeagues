using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.DTO;
public record LeagueDTO
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public string Name {get;set;} = "";
    public Country? Country {get;set;}
    public string Logo {get;set;} = "";
    public List<TeamDTO> Teams {get;set;} = new();
}