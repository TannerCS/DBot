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
            Commands = commandArray;
            Prefix = "!";
        }

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("guild_id")]
        public string guildID { get; set; }

        [BsonElement("commands")]
        public IList<CommandData> Commands { get; set; }

        [BsonElement("prefix")]
        public string Prefix { get; set; }
    }
}
