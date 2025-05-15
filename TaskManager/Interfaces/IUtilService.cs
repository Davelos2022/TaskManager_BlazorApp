namespace TaskManager.Interfaces
{
    public interface IUtilService
    {
        string FormatDate(DateTime date, string format = "dd.MM.yyyy");
        bool IsStrongPassword(string password);
        bool IsValidEmail(string email);
    }
}