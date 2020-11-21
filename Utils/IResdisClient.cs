using System;
using System.Threading.Tasks;

namespace NetNote.Utils
{
    public interface IResdisClient
    {
        string Get(string key);
        void Set(string key, object obj, int expireSec = 0);
        T Get<T>(string key) where T : new();
        Task<string> GetAsync(string key);
        Task SetAsync(string key, object obj, int expireSec = 0);
        Task<T> GetAsync<T>(string key) where T : new();
    }
}
