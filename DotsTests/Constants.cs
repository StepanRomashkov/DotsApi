using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DotsApi.Models;

namespace DotsTests
{
    public static class Constants
    {
        private static IConfigurationRoot _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

        private static MongoClient client = new MongoClient(GetMongoDbConnectionString());

        public static DotsDatabaseContext databaseContext = new DotsDatabaseContext(client, GetMongoDbDatabase());

        private static string GetSecret()
        {
            return _configuration.GetSection("AppSettings:Secret").Value;
        }

        private static string GetMongoDbConnectionString()
        {
            return _configuration.GetSection("MongoConnection:ConnectionString").Value;
        }

        private static string GetMongoDbDatabase()
        {
            //return _configuration.GetSection("MongoConnection:Database").Value;
            return "TestDotsDb";
        }
    }
}
