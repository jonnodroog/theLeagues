namespace Extensions
{
    public class AwaySettings
    {
        public string API_Key { get; set; } = "";
        public List<int> LeagueIdNumbers { get; set; } = new();
        public int? CurrentSeason { get; set; }
    }
}