namespace EasyInsur.Modules
{
    public interface IAppDialogService
    {
        void ShowError(string message, string title = "Error");
        void ShowWarning(string message, string title = "Warning");
        void ShowInfo(string message, string title = "Information");
        bool Confirm(string message, string title = "Confirm Action");
        void NotifySuccess(string message);
        void NotifyError(string message);
        void NotifyWarning(string message);
        void NotifyInfo(string message);
    }
}
