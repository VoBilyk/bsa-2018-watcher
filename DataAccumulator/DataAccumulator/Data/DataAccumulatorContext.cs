﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DataAccumulator.Entities;
using DataAccumulator.Models;

namespace DataAccumulator.Data
{
    public class DataAccumulatorContext
    {
        private readonly IMongoDatabase _database = null;

        public DataAccumulatorContext(string ConnectionString, string Database)
        {
            var client = new MongoClient(ConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(Database);
            }
        }

        public IMongoCollection<CollectedData> Datasets
        {
            get { return _database.GetCollection<CollectedData>("CollectedData"); }
        }
    }
}
