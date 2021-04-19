using Discord;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DBot.Constants
{
    public class GuildData
    {
        public GuildData(IGuild guild, List<CommandData> commandArray)
        {
            Id = ObjectId.GenerateNewId();
            guildID = guild.Id.ToString();
            ownerID = guild.OwnerId.ToString();
            Commands = commandArray;
            Prefix = "!";
            v = 0;
            Analytics = Database.GenerateAnalytics(guild);
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("guild_id")]
        public string guildID { get; set; }
        [BsonElement("owner_id")]
        public string ownerID { get; set; }

        [BsonElement("commands")]
        public IList<CommandData> Commands { get; set; }

        [BsonElement("prefix")]
        public string Prefix { get; set; }

        [BsonElement("analytics")]
        public AnalyticData Analytics { get; set; }

        [BsonElement("__v")]
        public int v { get; set; }
    }
}
