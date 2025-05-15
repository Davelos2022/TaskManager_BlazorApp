namespace TaskManager.Interfaces
{
    public interface ILoadingService
    {
        bool IsLoading { get; set; }
        event Action? OnLoadingChanged;
    }
}
