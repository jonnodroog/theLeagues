using System.Threading.Tasks;

namespace TheLeaguesUI.Services.Interfaces.Utilities
{
    public interface IClipboardService
    {
        Task SetTextAsync(string text);
        Task<string> GetTextAsync();
        Task ClearAsync();
    }
}