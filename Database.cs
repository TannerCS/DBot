using DBot.Constants;
using Discord;
using Discord.Commands;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace DBot
{
    public class Database
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

            return parsedGuildData.commands[commandName.ToLower()];
        }

        public BsonDocument InsertNewGuild(IGuild guild, IEnumerable<CommandInfo> commandInfo)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString());
            var guildData = _GuildInformation.Find(filter).FirstOrDefault();

            var doc = new BsonDocument {
                { "guild_id", guild.Id.ToString() },
                { "commands", new BsonDocument() }
            };

            if (guildData == null)
            {
                _GuildInformation.InsertOne(doc);
            }

            return doc;
        }

        public void UpdateGuildCommands(IGuild guild, IEnumerable<CommandInfo> commandInfo)
        {
            var parsedGuildData = GetGuildData(guild);

            var b = new BsonDocument();

            foreach (var command in commandInfo)
            {
                if (!parsedGuildData.commands.ContainsKey(command.Name.ToLower()))
                {
                    b.Add(command.Name.ToLower(), false);
                }
            }

            var set = Builders<BsonDocument>.Update.Set("commands", parsedGuildData.ToBsonDocument().GetElement("commands").Value.ToBsonDocument().AddRange(b));

            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
        }

        public void UpdateGuildCommand(IGuild guild, string command, bool enabled)
        {
            var parsedGuildData = GetGuildData(guild);
            parsedGuildData.commands[command] = enabled;

            var set = Builders<BsonDocument>.Update.Set("commands", parsedGuildData.ToBsonDocument().GetElement("commands").Value);

            _GuildInformation.UpdateOne(Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString()), set);
        }

        private GuildData GetGuildData(IGuild guild)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("guild_id", guild.Id.ToString());
            var guildData = _GuildInformation.Find(filter).FirstOrDefault();
            var parsedGuildData = BsonSerializer.Deserialize<GuildData>(guildData.ToString());

            return parsedGuildData;
        }
    }
}
