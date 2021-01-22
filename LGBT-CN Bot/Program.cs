using System;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using LGBTCN.Bot.Util;

namespace LGBTCN.Bot
{
    static class Program
    {
        private static TelegramBotClient _bot;

        public static async Task Main()
        {
            _bot = new TelegramBotClient(Configuration.BotToken);

            var me = await _bot.GetMeAsync();
            Console.Title = me.Username;

            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;
            _bot.OnReceiveError += BotOnReceiveError;

            _bot.StartReceiving(Array.Empty<UpdateType>());
            Log.I("main", $"Start listening for @{me.Username}");

            Console.ReadLine();
            _bot.StopReceiving();
            Log.I("main", "Stop receiving.");
        }


        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            if (!message.Text.StartsWith("/"))
                return;

            switch (message.Text.Split(' ').First())
            {
                case "/start":
                    await SendMsg(message, "Hello");
                    break;
                default:
                    await SendUsage(message);
                    break;
            }
        }
        static async Task SendUsage(Message message)
            => await SendMsg(message, Text.USAGE);

        static async Task SendMsg(Message message, string text)
        {
            await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Log.E("main.connect", $"{receiveErrorEventArgs.ApiRequestException.ErrorCode.ToString()}" +
                  " — " +
                  $"{receiveErrorEventArgs.ApiRequestException.Message}");
        }

    }
}
