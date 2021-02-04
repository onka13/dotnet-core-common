using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using CoreCommon.Infra.Helpers;
using CoreCommon.Data.MongoDBBase.Serializers;

namespace CoreCommon.Data.MongoDBBase.Base
{
    public abstract class MongoDbContextBase
    {
        /// <summary>
        /// Name of the context
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Autowired property for getting appsettings
        /// </summary>
        public IConfiguration Configuration { get; set; }

        IMongoDatabase _database;
        public IMongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    try
                    {
                        BsonSerializer.RegisterSerializer(new MyObjectSerializer());

                        // https://mongodb.github.io/mongo-csharp-driver/2.3/apidocs/html/N_MongoDB_Bson_Serialization_Conventions.htm
                        var conventionPack = new ConventionPack {
                        new IgnoreExtraElementsConvention(true),
                    };
                        ConventionRegistry.Register("pack", conventionPack, type => true);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                   
                    var databaseName = Configuration[Name + ":DatabaseName"];

                    if (!string.IsNullOrEmpty(Configuration[Name + "_ConnectionString"]))
                    {
                        databaseName = Configuration[Name + "_DatabaseName"];
                    }
                    _database = Client.GetDatabase(databaseName);
                }
                return _database;
            }
        }

        IMongoClient _client;
        protected IMongoClient Client
        {
            get
            {
                if (_client == null)
                {
                    var connectionString = Configuration[Name + ":ConnectionString"];

                    if (!string.IsNullOrEmpty(Configuration[Name + "_ConnectionString"]))
                    {
                        connectionString = Configuration[Name + "_ConnectionString"];
                    }

                    _client = new MongoClient(connectionString);
                }
                return _client;
            }
        }

        public string GetCollectionName<T>()
        {
            var name = "";

            var collectionAttribute = (CollectionAttribute)typeof(T).GetCustomAttributes(typeof(CollectionAttribute), false).FirstOrDefault();
            if (collectionAttribute != null) name = collectionAttribute.Name;
            else name = Regex.Replace(nameof(T), @"Entity$", "");

            return name;
        }
        
        public IMongoCollection<T> GetCollection<T>()
        {
            return Database.GetCollection<T>(GetCollectionName<T>());
        }
    }
}
