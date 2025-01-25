using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static readonly string BotToken = "7487072577:AAEtljRaSzEj9cYPUlbs82K163Qfwp_T_yU";
    private static readonly TelegramBotClient BotClient = new TelegramBotClient(BotToken);

    static async Task Main()
    {
        try
        {


            Console.WriteLine("Бот запущен!");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            BotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                cancellationToken: cancellationToken
            );

            Console.WriteLine("Нажмите Enter для завершения работы бота.");
            await Task.Run(() => Console.ReadLine());

            cts.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.InnerException}");
        }
    }


    // Обработчик обновлений (входящих сообщений)
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var message = update.Message;
            Console.WriteLine($"Получено сообщение: {message.Text} от {message.Chat.Id}");
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
                    Console.WriteLine($"Сохранено значение: {value.ToString()} по ключу: {item}");
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Сохранено значение: {value.ToString()} по ключу: {item}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Не удалось сохранить значение. Попробуйте еще раз.");
                }

            }
            else if (message.Text.ToLower().StartsWith("/send"))
            {
                //Send
                if (int.TryParse(messageParts[messagePartsLength - 1], out int value))
                {
                    var person = messageParts[2];
                    Console.WriteLine($"Сумма {value} была отправлена: {person}");
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Сумма {value} была отправлена: {person}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Не удалось отправить значение. Попробуйте еще раз.");
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда. Используйте команды /save и /get");
            }
        }
    }

    // Обработчик ошибок
    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}
