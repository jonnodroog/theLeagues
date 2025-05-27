using System.Threading.Tasks;

namespace TheLeaguesUI.Services.Interfaces
{
    public interface ILocalStorageService
    {
        Task SetItemAsync<T>(string key, T value);
        Task<T?> GetItemAsync<T>(string key);
        Task RemoveItemAsync(string key);
        Task ClearAsync();
        Task<bool> ContainsKeyAsync(string key);
    }
}