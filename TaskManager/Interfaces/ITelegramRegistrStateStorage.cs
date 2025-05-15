using TaskManager.Bots.Telegram;

namespace TaskManager.Interfaces
{
    public interface ITelegramRegistrStateStorage
    {
        Task<TelegramRegistrState?> GetStateAsync(long chatId);
        Task SaveStateAsync(TelegramRegistrState state);
        Task RemoveStateAsync(long chatId);
        Task<bool> StateExistsAsync(long chatId);
    }
}