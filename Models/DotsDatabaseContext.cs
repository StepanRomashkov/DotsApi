using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DotsApi.Models
{
    public class DotsDatabaseContext
    {
        private readonly IMongoDatabase _database = null;

        public DotsDatabaseContext(IOptions<DotsDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public DotsDatabaseContext(MongoClient client, string databaseName)
        {
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("Users");
            }
        }
    }
}
