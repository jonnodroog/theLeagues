namespace AwayAPI.Services.Interfaces
{
    public interface IPlayerService
    {
        public Task UpdateAllPlayers(CancellationToken ct);
        public Task UpdatePlayerById(int id,CancellationToken ct);
    }
}