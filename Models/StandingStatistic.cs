namespace Models;

public record StandingStatistic
{
    public int Played {get;set;}
    public int  Win {get;set;}
    public int Draw {get;set;}
    public int Lose {get;set;}
    public Goals Goals {get;set;} = new();
}