namespace Models;
public record League
{
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Country {get;set;} = "";
    public string Logo {get;set;} = "";
    public string Flag {get;set;} = "";
    public int? Season {get;set;}
    public List<List<Standing>>? Standings {get;set;} 
}