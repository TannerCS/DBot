using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DBot.Services
{
    public class LoggingService
    {
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += LogAsync;
            command.Log += LogAsync;
        }
        public Task LogAsync(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Info:
                case LogSeverity.Debug:
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
            {
                Console.WriteLine($"[General/{message.Severity}] {message}");
            }

            Console.ResetColor();

            return Task.CompletedTask;
        }

        public void Log(LogMessage message)
        {
            LogAsync(message);
        }
    }
}