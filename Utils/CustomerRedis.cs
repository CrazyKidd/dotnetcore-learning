using System;
using System.Threading.Tasks;
using CSRedis;

namespace NetNote.Utils
{
    public class CustomerRedis : IResdisClient
    {
        public CustomerRedis()
        {
            CSRedisClient csredis = new CSRedis.CSRedisClient("127.0.0.1:6379,defaultDatabase=1,poolsize=50,ssl=false,writeBuffer=10240");
            RedisHelper.Initialization(csredis);
        }

        public string Get(string key)
        {
            //throw new NotImplementedException();
            return RedisHelper.Get(key);
        }

        public T Get<T>(string key) where T : new()
        {
            //throw new NotImplementedException();
            return RedisHelper.Get<T>(key);
        }

        public async Task<string> GetAsync(string key)
        {
            //throw new NotImplementedException();
            return await RedisHelper.GetAsync(key);
        }

        public async Task<T> GetAsync<T>(string key) where T : new()
        {
            //throw new NotImplementedException();
            return await RedisHelper.GetAsync<T>(key);
        }

        public void Set(string key, object obj, int expireSec = 0)
        {
            //throw new NotImplementedException();
            RedisHelper.Set(key, obj, expireSec);
        }

        public async Task SetAsync(string key, object obj, int expireSec = 0)
        {
            //throw new NotImplementedException();
            await RedisHelper.SetAsync(key, obj, expireSec);
        }
    }
}
