namespace Models;
public record League
{
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Logo {get;set;} = "";
    public string Flag {get;set;} = "";
}