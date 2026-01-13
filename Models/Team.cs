using Models.DTO;

namespace Models;
public record Team
{
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Code {get;set;} = "";
    public Country? Country {get;set;}
    public DateTime? Founded {get;set;}
    public bool National {get;set;}
    public string Logo {get;set;} = "";
}