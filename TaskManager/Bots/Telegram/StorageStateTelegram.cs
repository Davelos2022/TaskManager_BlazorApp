using System.Collections.Concurrent;
using TaskManager.Interfaces;

namespace TaskManager.Bots.Telegram
{
    public class StorageStateTelegram : ITelegramRegistrStateStorage
    {
        private readonly ConcurrentDictionary<long, TelegramRegistrState> _states = new();

        public Task<TelegramRegistrState?> GetStateAsync(long chatId)
        {
            if (_states.TryGetValue(chatId, out var state))
            {
                if (state.IsExpired)
                {
                    _states.TryRemove(chatId, out _);
                    return Task.FromResult<TelegramRegistrState?>(null);
                }
                return Task.FromResult<TelegramRegistrState?>(state);
            }
            return Task.FromResult<TelegramRegistrState?>(null);
        }

        public Task SaveStateAsync(TelegramRegistrState state)
        {
            _states[state.ChatId] = state;
            return Task.CompletedTask;
        }

        public Task RemoveStateAsync(long chatId)
        {
            _states.TryRemove(chatId, out _);
            return Task.CompletedTask;
        }

        public Task<bool> StateExistsAsync(long chatId)
        {
            return Task.FromResult(_states.ContainsKey(chatId));
        }
    }
}
