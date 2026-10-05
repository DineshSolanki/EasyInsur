using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using EasyInsur.Models;
using EasyInsur.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace EasyInsur.ViewModels
{
    public class DashboardViewModel : BindableBase
    {
        private readonly IRegionManager? _regionManager;

        #region Metric Properties
        private long _customersCount;
        public long CustomersCount
        {
            get => _customersCount;
            set => SetProperty(ref _customersCount, value);
        }

        private long _agentsCount;
        public long AgentsCount
        {
            get => _agentsCount;
            set => SetProperty(ref _agentsCount, value);
        }

        private long _totalPeople;
        public long TotalPeople
        {
            get => _totalPeople;
            set => SetProperty(ref _totalPeople, value);
        }

        private double _totalPremium;
        public double TotalPremium
        {
            get => _totalPremium;
            set => SetProperty(ref _totalPremium, value);
        }

        private double _totalCommission;
        public double TotalCommission
        {
            get => _totalCommission;
            set => SetProperty(ref _totalCommission, value);
        }

        private double _totalBalance;
        public double TotalBalance
        {
            get => _totalBalance;
            set => SetProperty(ref _totalBalance, value);
        }

        private long _transactionCount;
        public long TransactionCount
        {
            get => _transactionCount;
            set => SetProperty(ref _transactionCount, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _lastUpdatedText = "Just now";
        public string LastUpdatedText
        {
            get => _lastUpdatedText;
            set => SetProperty(ref _lastUpdatedText, value);
        }

        private ObservableCollection<DashboardActivityItem> _recentActivities = new();
        public ObservableCollection<DashboardActivityItem> RecentActivities
        {
            get => _recentActivities;
            set => SetProperty(ref _recentActivities, value);
        }

        private Dictionary<string, string> _links = new();
        public Dictionary<string, string> Links
        {
            get => _links;
            set => SetProperty(ref _links, value);
        }

        public string OwnerName => Services.Settings?.OwnerName ?? "Insurance Agency";
        public string OwnerPhone => Services.Settings?.OwnerPhone ?? "-";
        public string OwnerEmail => Services.Settings?.OwnerEmail ?? "-";
        #endregion

        #region Commands
        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand<string> NavigateCommand { get; }
        public DelegateCommand<string> OpenUrlCommand { get; }
        #endregion

        public DashboardViewModel() : this(null)
        {
        }

        public DashboardViewModel(IRegionManager? regionManager)
        {
            _regionManager = regionManager;

            RefreshCommand = new DelegateCommand(async () => await LoadDashboardDataAsync());
            NavigateCommand = new DelegateCommand<string>(viewName =>
            {
                if (!string.IsNullOrWhiteSpace(viewName) && _regionManager != null)
                {
                    _regionManager.RequestNavigate("ContentRegion", viewName);
                }
            });

            OpenUrlCommand = new DelegateCommand<string>(url =>
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    try
                    {
                        Util.StartProcess(url);
                    }
                    catch
                    {
                        // Ignore invalid or blocked URLs
                    }
                }
            });

            Links = new Dictionary<string, string>
            {
                { "General Insurance Council", "https://www.gicouncil.in" },
                { "All India RTO codes list", Util.Rtolistpdf },
                { "Parivahan Sewa Portal", "https://parivahan.gov.in/" },
                { "Vahan National Register", "https://vahan.nic.in/" }
            };

            _ = LoadDashboardDataAsync();
        }

        public async Task LoadDashboardDataAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;

                var metrics = await DbMethods.GetDashboardMetricsAsync();
                CustomersCount = metrics.customerCount;
                AgentsCount = metrics.agentCount;
                TotalPeople = metrics.customerCount + metrics.agentCount;
                TotalPremium = metrics.totalPremium;
                TotalCommission = metrics.totalCommission;
                TotalBalance = metrics.totalBalance;
                TransactionCount = metrics.transactionCount;

                var activities = await DbMethods.GetRecentDashboardActivityAsync(10);
                RecentActivities = new ObservableCollection<DashboardActivityItem>(activities);

                LastUpdatedText = DateTime.Now.ToString("hh:mm:ss tt");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard metrics: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
