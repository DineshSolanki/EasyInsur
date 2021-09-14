using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace EasyInsur.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        private string _title = "BatchMuxer_Subtitle";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public DelegateCommand<FunctionEventArgs<object>> NavigateCommand { get; }
        public DelegateCommand<string> SelectCmd { get; }
        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<FunctionEventArgs<object>>(Navigate);
            SelectCmd = new DelegateCommand<string>( i => _regionManager.RequestNavigate("ContentRegion", i));
        }
        private void Navigate(FunctionEventArgs<object> functionEventArgs)
        {
            var navigatePath = (functionEventArgs.Info as SideMenuItem)?.Tag?.ToString();
            if(navigatePath.IsNullOrEmpty()) return;
            _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }
    }
}
