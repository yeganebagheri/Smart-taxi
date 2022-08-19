using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
//using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IRedisServices
    {
        Task<string> GetFromRedis(Guid redisKey);
        Task SetInRedis(Guid redisKey, string value);
    }

    public class RedisServices : IRedisServices
    {
        private readonly IDistributedCache _cache;
        //private readonly IConnectionMultiplexer _connectionMultiplexer;
        public  RedisServices(IDistributedCache cache /*, IConnectionMultiplexer connectionMultiplexer*/)
        {
            _cache = cache;
            //_connectionMultiplexer = connectionMultiplexer;

        }

        public async Task<string> GetFromRedis(Guid redisKey)
        {
            string serializedCustomerList;
            var stringKey = redisKey.ToString();
            var redisCustomerList = await _cache.GetAsync(stringKey);
            serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);
            return JsonConvert.DeserializeObject<string>(serializedCustomerList);  
            
        }

        public async Task SetInRedis(Guid redisKey , string value)
        {
            string serializedCustomerList;
            var stringKey = redisKey.ToString();
            var redisCustomerList = await _cache.GetAsync(stringKey);

            if (redisCustomerList != null)
            {
                serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);
                var weatherList = JsonConvert.DeserializeObject<List<string>>(serializedCustomerList);
            }
            else
            {
               // var weatherList = _connectionMultiplexer.GetDatabase();
                var val = JsonConvert.SerializeObject(value);
                var byteValue = Encoding.UTF8.GetBytes(val);
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await _cache.SetAsync(stringKey, byteValue, options);

            }

        }

    }

}
