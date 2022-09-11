using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using EasyInsur.Models;
using EasyInsur.Modules;
using Prism.Regions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;

namespace EasyInsur.ViewModels
{
    public class ViewPeopleWindowViewModel : BindableBase
    {
        public ViewPeopleWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            AllPeople = DbMethods.GetPeople();
            PersonType = PersonType.Any;
            ReloadCommand = new DelegateCommand(ReloadMethod);
            SaveCommand = new DelegateCommand(SaveMethod);
            AddPayment = new DelegateCommand<object>(PerformAddPayment);
        }

        private void ReloadMethod()
        {
            AllPeople = DbMethods.GetPeople();
            PersonType = _personType;
        }

        #region Properties
        private readonly IRegionManager _regionManager;

        private bool _allowOutlining = true;
        public bool AllowOutlining { get => _allowOutlining; set => SetProperty(ref _allowOutlining, value); }

        private bool _exportCurrentPageOnly = true;
        public bool ExportCurrentPageOnly { get => _exportCurrentPageOnly; set => SetProperty(ref _exportCurrentPageOnly, value); }
        private PersonType _personType;
        public PersonType PersonType
        {
            get => _personType;
            set
            {
                SetProperty(ref _personType, value);
                People = PersonType switch
                {
                    PersonType.Agent => Agents,
                    PersonType.Customer => Customers,
                    _ => AllPeople
                };
            }
        }

        private IEnumerable<Person> _allPeople;
        private IEnumerable<Person> AllPeople
        {
            get => _allPeople;
            set
            {
                SetProperty(ref _allPeople, value);
                var enumerable = _allPeople.ToList();
                Agents = enumerable.Where(p => p.Type == "Agent");
                Customers = enumerable.Where(p => p.Type == "Customer");
            }
        }
        private IEnumerable<Person> Agents { get; set; }
        private IEnumerable<Person> Customers { get; set; }
        private IEnumerable<Person> _people;
        public IEnumerable<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }
        #endregion

        #region Delegates

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand<object> AddPayment { get; }

        private void PerformAddPayment(object commandParameter)
        {
            if (commandParameter is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                var parameters = new NavigationParameters
                {
                    { "person", grid.SelectedItem as Person}
                };
                _regionManager.RequestNavigate("ContentRegion", "PaymentWindow", parameters);
            }
        }

        #endregion

        private void SaveMethod()
        {
            var r = AllPeople.Where(p => p.EditedColumns.Any());
            var enumerable = r.ToList();
            if (!enumerable.Any()) return;
            DbMethods.UpdatePeople(enumerable);
            ReloadMethod();
            //foreach (var person in AllPeople.Where(p => p.EditedColumns.Any()))
            //{
            //    DBMethods.UpdatePerson(person);
            //}
        }
    }
}
