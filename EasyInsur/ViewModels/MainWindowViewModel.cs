using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Specialized;

namespace EasyInsur.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        private string _title = "EasyInsur - Insurance Management";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _currentView = "Dashboard";
        public string CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private bool _isSidebarCollapsed = false;
        public bool IsSidebarCollapsed
        {
            get => _isSidebarCollapsed;
            set
            {
                if (SetProperty(ref _isSidebarCollapsed, value))
                {
                    RaisePropertyChanged(nameof(SidebarWidth));
                    RaisePropertyChanged(nameof(IsSidebarExpanded));
                }
            }
        }

        public bool IsSidebarExpanded => !IsSidebarCollapsed;
        public double SidebarWidth => IsSidebarCollapsed ? 68 : 230;

        public string OwnerName => Services.Settings?.OwnerName ?? "Agency Admin";

        public DelegateCommand<FunctionEventArgs<object>> NavigateCommand { get; }
        public DelegateCommand<string> SelectCmd { get; }
        public DelegateCommand ToggleSidebarCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            ToggleSidebarCommand = new DelegateCommand(() => IsSidebarCollapsed = !IsSidebarCollapsed);

            SelectCmd = new DelegateCommand<string>(path =>
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    CurrentView = path;
                    _regionManager.RequestNavigate("ContentRegion", path);
                }
            });

            NavigateCommand = new DelegateCommand<FunctionEventArgs<object>>(Navigate);

            // Synchronize CurrentView with Prism region navigation events
            ((INotifyCollectionChanged)_regionManager.Regions).CollectionChanged += OnRegionsCollectionChanged;
        }

        private void OnRegionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_regionManager.Regions.ContainsRegionWithName("ContentRegion"))
            {
                var region = _regionManager.Regions["ContentRegion"];
                region.NavigationService.Navigated += (s, args) =>
                {
                    var uri = args.Uri?.OriginalString ?? args.Uri?.ToString();
                    if (!string.IsNullOrEmpty(uri))
                    {
                        CurrentView = uri;
                    }
                };
                ((INotifyCollectionChanged)_regionManager.Regions).CollectionChanged -= OnRegionsCollectionChanged;
            }
        }

        private void Navigate(FunctionEventArgs<object> functionEventArgs)
        {
            var navigatePath = (functionEventArgs.Info as SideMenuItem)?.Tag?.ToString();
            if (string.IsNullOrWhiteSpace(navigatePath)) return;
            CurrentView = navigatePath;
            _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }
    }
}
