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

        private static Translate _translateEN = new Translate(Translate.Language.Auto, Translate.Language.English);

        private static Translate _translateZH = new Translate(Translate.Language.Auto, Translate.Language.Chinese);

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

            if (!Configuration.ALLOW_USERNAME.Contains(message.Chat.Username) &&
                !Configuration.DEBUGGER_ID.Contains(message.From.Id))
            {
                Log.E("main.msg", $"Not allowed group! {{\"id\":{message.Chat.Id}," +
                    $"\"title\":\"{message.Chat.Title}\"," +
                    $"\"username\":\"{message.Chat.Username}\"," +
                    $"\"type\":\"{message.Chat.Type}\"," +
                    $"}}");
                return;
            }

            if (message == null || message.Type != MessageType.Text)
                return;

            if (!message.Text.StartsWith("/"))
                return;

            switch (message.Text.Split(' ').First())
            {
                case "/translate":
                    await SendTranslate(message);
                    break;
                case "/start":
                    await SendMsg(message, "Hello");
                    break;
                case "/debug":
                    await SendDebug(message);
                    break;
                case "/about":
                    await SendMsg(message, Text.ABOUT);
                    break;
                default:
                    await SendUsage(message);
                    break;
            }
        }

        static async Task SendDebug(Message message)
        {
            string replyStr = "NO PERMISSION!";
            if (message.From.Id == 573387497)
            {
                replyStr = $"CHAT.ID {message.Chat.Id}\n" +
                    $"CHAT.UN {message.Chat.Username}\n" +
                    $"CHAT.TTL {message.Chat.Title}";
            }
            await _bot.SendTextMessageAsync(
                    replyToMessageId: message.MessageId,
                    chatId: message.Chat.Id,
                    text: replyStr,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            return;
        }

        static async Task SendTranslate(Message message)
        {
            string replyStr;
            int replyMsgId;
            if (message.ReplyToMessage == null)
            {
                replyStr = "The message shouldn't be empty!";
                replyMsgId = message.MessageId;
            }
            else
            {
                var text = message.ReplyToMessage.Text;
                replyStr = await Task.Run(() => GetTransMsg(text));
                replyMsgId = message.ReplyToMessage.MessageId;
            }
            await _bot.SendTextMessageAsync(
                    replyToMessageId: replyMsgId,
                    chatId: message.Chat.Id,
                    text: replyStr,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            return;
        }

        static string GetTransMsg(string text)
        {
            try
            {
                string result = $"Orgin: {text}\n";
                text = text.Trim();
                var en = _translateEN.Text(text).Trim();
                var zh = _translateZH.Text(text).Trim();
                if (text != en)
                    result += "EN: " + en + "\n";
                if (text != zh)
                    result += "ZH: " + zh + "\n";
                return result.Trim();
            }
            catch (Exception ex)
            {
                Log.E("main.translate", ex.ToString());
                return Text.OOPS;
            }
        }

        static async Task SendUsage(Message message)
            => await SendMsg(message, Text.USAGE);

        static async Task SendMsg(Message message, string text, int replyToMessageId = 0)
        {
            await _bot.SendTextMessageAsync(
                replyToMessageId: replyToMessageId,
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Log.E("main.connect", $"{receiveErrorEventArgs.ApiRequestException.ErrorCode.ToString()}" +
                  " - " +
                  $"{receiveErrorEventArgs.ApiRequestException.Message}");
        }

    }
}
