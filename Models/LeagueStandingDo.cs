namespace Models;
public record LeagueStandingDo
{
    //Duplication of League class needed here because third party API that provides the data has Country as a string for when we call the standing endpoint which is needed to seed teams data.
    public LeagueStanding? League {get;set;} //this just what its called in the body
}

