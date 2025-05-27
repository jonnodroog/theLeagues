using System.Threading.Tasks;

namespace TheLeaguesUI.Services.Interfaces
{
    public interface IClipboardService
    {
        Task SetTextAsync(string text);
        Task<string> GetTextAsync();
        Task ClearAsync();
    }
}