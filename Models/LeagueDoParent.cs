namespace Models;
    public record LeagueDoParent
    {
        public required List<LeagueDo> Response {get;set;} = new();
    }