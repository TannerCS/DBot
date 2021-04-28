using Discord;
using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBot.Constants
{
    public class AnalyticData
    {
        public AnalyticData(IGuild guild)
        {
            int memberCount = (guild as SocketGuild).DownloadedMemberCount;
            ApproximateMemberCount = new List<Analytic>() { new Analytic() { Timestamp = DiscordBot.Database.GetCurrentUnixTimestamp(), Count = memberCount } };
            Name = guild.Name;
            PremiumSubscriptionCount = guild.PremiumSubscriptionCount;
        }

        public AnalyticData() { }

        [BsonElement("messages_received")]
        public List<Analytic> MessagesReceived { get; set; }

        [BsonElement("messages_deleted")]
        public List<Analytic> MessagesDeleted { get; set; }

        [BsonElement("invites_created")]
        public List<Analytic> InvitesCreated { get; set; }

        [BsonElement("invites_deleted")]
        public List<Analytic> InvitesDeleted { get; set; }

        [BsonElement("user_banned")]
        public List<Analytic> UsersBanned { get; set; }

        [BsonElement("users_joined")]
        public List<Analytic> UsersJoined { get; set; }

        [BsonElement("users_unbanned")]
        public List<Analytic> UsersUnbanned { get; set; }

        [BsonElement("users_left")]
        public List<Analytic> UsersLeft { get; set; }

        [BsonElement("approximate_member_count")]
        public List<Analytic> ApproximateMemberCount { get; set; }

        [BsonElement("approximate_presence_count")]
        public List<Analytic> ApproximatePresenceCount { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("premium_subscription_count")]
        public int PremiumSubscriptionCount { get; set; }

        public class Analytic
        {
            public Analytic()
            {
                Timestamp = DiscordBot.Database.GetCurrentUnixTimestamp();
                Count = 0;
            }

            public double Timestamp { get; set; }
            public int Count { get; set; }
        }
    }
}
