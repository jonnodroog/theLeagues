namespace Models;
public record LeagueStanding
{

    //Duplication of League class needed here because third party API that provides the data has Country as a string for when we call the standing endpoint which is needed to seed teams data.
    public int Id {get;set;}
    public string Name {get;set;} = "";
    public string Logo {get;set;} = "";
    public string? Country {get; set;}
    public List<List<Standing>>? Standings {get;set;}
}