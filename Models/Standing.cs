namespace Models;

public record Standing 
{
    public int Rank {get;set;}
    public Team? Team {get;set;}
    public int? Points {get;set;}
    public int? GoalsDiff {get;set;}
    public string Group {get;set;}= "";
    public string Form {get;set;}= "";
    public string Status {get;set;}= "";
    public string Description {get;set;} = "";
    public StandingStatistic All {get;set;} = new();
    public StandingStatistic? Home {get;set;}
    public StandingStatistic? Away {get;set;}
    public DateTime? Update {get;set;}
}