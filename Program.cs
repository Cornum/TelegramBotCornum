using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static readonly string BotToken = "token";
    private static readonly TelegramBotClient BotClient = new TelegramBotClient(BotToken);

    static async Task Main()
    {
        try
        {


            Console.WriteLine("Bot started");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            BotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                cancellationToken: cancellationToken
            );

            Console.WriteLine("Enter to stop bot.");
            await Task.Run(() => Console.ReadLine());

            cts.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.InnerException}");
        }
    }


    // Update handler (incoming messages)
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var message = update.Message;
            Console.WriteLine($"Got message: {message.Text} from {message.Chat.Id}");
            var messageParts = message.Text.Split(' ');
            int messagePartsLength = messageParts.Length;

            if (message.Text.ToLower().StartsWith("/save"))
            {
                //Save
                if (int.TryParse(messageParts[messagePartsLength - 1], out int value))
                {
                    var item = "";
                    for (int i = 1; i < messagePartsLength - 1; i++)
                    {
                        item += messageParts[i] + " "; // Save in dictionary
                    }
                    Console.WriteLine($"Saved value: {value.ToString()} by key: {item}");
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Saved value: {value.ToString()} by key: {item}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Saving has failed. Try again.");
                }

            }
            else if (message.Text.ToLower().StartsWith("/send"))
            {
                //Send
                if (int.TryParse(messageParts[messagePartsLength - 1], out int value))
                {
                    var person = messageParts[2];
                    Console.WriteLine($"{value} was sent to: {person}");
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{value} was sent to: {person}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Sending has failed. Try again.");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Unknown command. Use commands /save and /send");
            }
        }
    }

    // Error handler
    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception.Message}");
        return Task.CompletedTask;
    }
}
