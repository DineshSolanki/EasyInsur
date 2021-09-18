using System.Collections.Generic;
using EasyInsur.Modules;
using Prism.Mvvm;

namespace EasyInsur.ViewModels
{
    public class DashboardViewModel : BindableBase
    {
        #region Properties

        

        
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

        private Dictionary<string, string> _links;

        public Dictionary<string, string> Links
        {
            get => _links;
            set => SetProperty(ref _links, value);
        }
        #endregion
        public DashboardViewModel()
        {
            DbMethods.GetAgentCount().ContinueWith(ac =>
            {
                AgentsCount = ac.Result;
            });
            DbMethods.GetCustomerCount().ContinueWith(cc =>
            {
                CustomersCount = cc.Result;
            });
            Links = new Dictionary<string, string>
            {
                { "General Insurance Council", "https://www.gicouncil.in" },
                {"All India RTO codes list", Util.Rtolistpdf},
                {"Parivahan Sewa","https://parivahan.gov.in/"},
                {"Vahan National Register","https://vahan.nic.in/"}

            };
        }
    }
}
