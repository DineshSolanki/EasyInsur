using System.Windows;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace EasyInsur.Modules
{
    public class AppDialogService : IAppDialogService
    {
        private static IAppDialogService? _current;
        public static IAppDialogService Current => _current ??= new AppDialogService();

        public void ShowError(string message, string title = "Error")
        {
            MessageBox.Error(message, title);
        }

        public void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Warning(message, title);
        }

        public void ShowInfo(string message, string title = "Information")
        {
            MessageBox.Info(message, title);
        }

        public bool Confirm(string message, string title = "Confirm Action")
        {
            return MessageBox.Ask(message, title) == MessageBoxResult.OK;
        }

        public void NotifySuccess(string message)
        {
            Growl.Success(message);
        }

        public void NotifyError(string message)
        {
            Growl.Error(message);
        }

        public void NotifyWarning(string message)
        {
            Growl.Warning(message);
        }

        public void NotifyInfo(string message)
        {
            Growl.Info(message);
        }
    }
}
