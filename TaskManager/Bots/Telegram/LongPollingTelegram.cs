using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;

namespace TaskManager.Bots.Telegram
{
    public class LongPollingTelegram : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceScopeFactory _scopeFactory;

        public LongPollingTelegram(ITelegramBotClient botClient, IServiceScopeFactory scopeFactory)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions { AllowedUpdates = { }, };

            try
            {
                await _botClient.ReceiveAsync(
                    updateHandler: async (botClient, update, cancellationToken) =>
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var updateHandler = scope.ServiceProvider.GetRequiredService<TelegramBot>();
                            await updateHandler.HandleUpdateAsync(update);
                        }
                    },
                    errorHandler: HandleErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Error:\n[{apiRequestException.ErrorCode}]" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
