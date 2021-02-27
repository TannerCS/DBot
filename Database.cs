using DBot.Constants;
using DBot.Services;
using Discord;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBot
{
    class Database
    {
        private MongoClient _DBClient;
        private IMongoDatabase _Database;
        private IMongoCollection<BsonDocument> _GuildInformation;

        public Database()
        {
            _DBClient = new MongoClient(DiscordBot.Config.Data.mongoUrl);

            _Database = _DBClient.GetDatabase("discord-bot");
            _GuildInformation = _Database.GetCollection<BsonDocument>("guild-information");
        }

        public bool IsCommandEnabledForGuild(IGuild guild, string commandName)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString());
            var guildData = _GuildInformation.Find(filter).FirstOrDefault();

            if (guildData == null) return false;

            var parsedGuildData = BsonSerializer.Deserialize<GuildData>(guildData.ToString());

            return parsedGuildData.commands[commandName];
        }

        public void InsertNewGuild(IGuild guild)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString());
            var guildData = _GuildInformation.Find(filter).FirstOrDefault();

            if (guildData == null)
            {
                _GuildInformation.InsertOne(new BsonDocument {
                    { "guild_id", guild.Id.ToString() },
                    { "commands", new BsonDocument { { "ping", true } } }
                });
            }
        }
    }
}
