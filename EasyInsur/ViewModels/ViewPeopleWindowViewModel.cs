using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using EasyInsur.Models;
using EasyInsur.Modules;

namespace EasyInsur.ViewModels
{
    public class ViewPeopleWindowViewModel : BindableBase
    {
        public ViewPeopleWindowViewModel()
        {
            AllPeople = DbMethods.GetPeople();
            PersonType = PersonType.Any;
            ReloadCommand = new DelegateCommand(ReloadMethod);
            SaveCommand = new DelegateCommand(SaveMethod);
        }

        private void ReloadMethod()
        {
            AllPeople = DbMethods.GetPeople();
            PersonType = _personType;
        }

        #region Properties

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
