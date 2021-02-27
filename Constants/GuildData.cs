using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DBot.Constants
{
    class GuildData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("guild_id")]
        public ulong guildID { get; set; }
        [BsonElement("commands")]
        public Dictionary<string, bool> commands = new Dictionary<string, bool>();
    }
}
