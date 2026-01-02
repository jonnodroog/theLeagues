namespace AwayAPI.Services.Interfaces
{
    public interface IPlayerService
    {
        public Task UpdateAllPlayers();
        public Task UpdatePlayer(int id);
    }
}