﻿using DBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DBot
{
    class DiscordBot
    {
        public static Config Config;
        public static Database Database;
        public static LoggingService Logger;

        private static DiscordSocketClient _Client;
        private static CommandHandler _Command;
        private static CommandService _CommandService;
        private static MessageService _MessageService;
        public async Task MainAsync()
        {

            _Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                ExclusiveBulkDelete = true,
                LogLevel = LogSeverity.Verbose,
                MaxWaitBetweenGuildAvailablesBeforeReady = 5
            });

            _CommandService = new CommandService();

            _Command = new CommandHandler(_Client, _CommandService);
            Logger = new LoggingService(_Client, _CommandService);
            _MessageService = new MessageService(_Client);
            Config = new Config();
            Database = new Database();

            _Client.Ready += Ready;

            await _Command.InstallCommandsAsync();

            //TODO: Secure this
            var token = Config.Data.botToken;

            await _Client.LoginAsync(TokenType.Bot, token);
            await _Client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Ready()
        {
            foreach (var guild in _Client.Guilds)
            {
                Database.InsertNewGuild(guild);
            }

            return Task.CompletedTask;
        }

        public static int GetLatency()
        {
            return _Client.Latency;
        }
    }
}