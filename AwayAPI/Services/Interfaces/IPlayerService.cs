namespace AwayAPI.Services.Interfaces
{
    public interface IPlayerService
    {
        public Task UpdateAllPlayers(CancellationToken ct);
        public Task UpdatePlayer(int id,CancellationToken ct);
    }
}