using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ServiceStack;
using ServiceStack.Redis;
using System.Data.Common;

namespace BikeStore.Helper
{
    public interface IRedisHelper
    {
        T Exists<T>(string key);
        void Write<T>(string key, dynamic value, DateTime expireDate);
        void Write<T>(string key, dynamic value);
        public bool Remove(string key);
    }

    public class RedisHelper : IRedisHelper
    {
        RedisEndpoint redisEndpoint;

        public RedisHelper()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = configuration.GetSection("RedisConnection").Value;

            int port = int.Parse((string) builder["port"]);
            string host = (string) builder["host"];
            string password = (string) builder["password"];

            redisEndpoint = new RedisEndpoint(host, port, password: password);
        }

        public T Exists<T>(string key)
        {
            using (var client = new RedisClient(redisEndpoint))
            {
                return client.Get<T>(key);
            }
        }

        public void Write<T>(string key, dynamic value)
        {
            using (var client = new RedisClient(redisEndpoint))
            {
                client.Set<T>(key, value);
            }
        }

        public void Write<T>(string key, dynamic value, DateTime expireDate)
        {
            using (var client = new RedisClient(redisEndpoint))
            {
                client.Set<T>(key, value, expireDate);
            }
        }

        public bool Remove(string key)
        {
            using (var client = new RedisClient(redisEndpoint))
            {
                return client.Remove(key);
            }
        }
    }
}

