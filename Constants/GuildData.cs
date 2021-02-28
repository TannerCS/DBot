using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DBot.Constants
{
    public class GuildData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("guild_id")]
        public ulong guildID { get; set; }

        [BsonElement("commands")]
        public IList<CommandData> Commands { get; set; }

        [BsonElement("prefix")]
        public string Prefix = "!";
    }
}
