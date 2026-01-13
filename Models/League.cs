namespace Models;
public record League
{
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Logo {get;set;} = "";
    public Country? Country {get; set;}
    public List<List<Standing>>? Standings {get;set;}
}