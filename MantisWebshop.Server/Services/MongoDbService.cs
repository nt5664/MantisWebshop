using MantisWebshop.Server.Models;
using MongoDB.Driver;
using System.Diagnostics;

namespace MantisWebshop.Server.Services
{
    public class MongoDbService
    {
        #region Static members
        private static MongoDbService? _instance;

        public static MongoDbService Instance => _instance!;

        public static void Initialize()
        {
            _instance = new MongoDbService();
        }
        #endregion

        private readonly MongoClient _client;

        private MongoDbService()
        {
            _client = new MongoClient("mongodbconnectionstringgoeshere");
        }

        public IMongoDatabase Db => _client.GetDatabase(Constants.DB_NAME);
    }
}
