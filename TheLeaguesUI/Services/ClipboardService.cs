using System.Threading.Tasks;
using Microsoft.JSInterop;
using TheLeaguesUI.Services.Interfaces.Utilities;

namespace TheLeaguesUI.Services
{
    public class ClipboardService : IClipboardService
    {
        private readonly IJSRuntime _jsRuntime;

        public ClipboardService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetTextAsync(string text)
        {
            await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }

        public async Task<string> GetTextAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
        }

        public async Task ClearAsync()
        {
            // Clipboard API does not have a direct clear method, so set empty text
            await SetTextAsync(string.Empty);
        }
    }
}
