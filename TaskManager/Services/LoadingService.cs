using TaskManager.Interfaces;

namespace TaskManager.Services
{
    public class LoadingService : ILoadingService
    {
        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnLoadingChanged?.Invoke();
                }
            }
        }

        public event Action? OnLoadingChanged;
    }
}