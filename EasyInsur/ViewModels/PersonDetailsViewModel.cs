using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyInsur.Models;
using EasyInsur.Modules;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RepoDb;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid;

namespace EasyInsur.ViewModels
{
    public class PersonDetailsViewModel : BindableBase
    {
        public PersonDetailsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            CountryDetails = new ObservableCollection<Country>(Util.Read()!.OrderBy(c => c.name));
            ResetCommand = new DelegateCommand(ResetFields);
            SaveCommand = new DelegateCommand(Save);
            ResetFields();
            PersonId = DbMethods.GetPeopleLastIdSync().ToString();
            LoadPeople();
        }

        private void LoadPeople()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var people = connection.QueryAll<Person>().ToList();
            PersonData = people;
        }

        private string _mobileMask;
        public string MobileMask
        {
            get => _mobileMask;
            set => SetProperty(ref _mobileMask, value);
        }
        private IEnumerable<Person> _people;
        public IEnumerable<Person> PersonData { get => _people; set => SetProperty(ref _people, value); }
        private ObservableCollection<Country> _countryDetails;
        private readonly IRegionManager _regionManager;

        public ObservableCollection<Country> CountryDetails
        {
            get => _countryDetails;
            set => SetProperty(ref _countryDetails, value);
        }

        private string _personType;
        public string PersonType
        {
            get => _personType;
            set
            {
                SetProperty(ref _personType, value);
                if (value is null) return;
                if (PersonType == "Agent")
                    _ = LoadPeopleAsync(DbMethods.LoadAgents);
                else
                    _ = LoadPeopleAsync(DbMethods.LoadCustomers);
            }
        }

        private async Task LoadPeopleAsync(Func<Task<List<Person>>> loader)
        {
            try
            {
                PersonData = await loader();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private string _personId;
        public string PersonId
        {
            get => _personId;
            set => SetProperty(ref _personId, value);
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private DateTime _registrationDate = DateTime.Today;
        public DateTime RegistrationDate
        {
            get => _registrationDate;
            set => SetProperty(ref _registrationDate, value);
        }

        private double _balance;
        public double Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }
        private string _mobile;
        public string Mobile
        {
            get => _mobile;
            set => SetProperty(ref _mobile, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _imagePathTag = "";
        public string ImagePathTag
        {
            get => _imagePathTag;
            set => SetProperty(ref _imagePathTag, value);
        }
        private Country _selectedCountry;

        public Country SelectedCountry
        {
            get => _selectedCountry;
            set => SetProperty(ref _selectedCountry, value);
        }

        public DelegateCommand ResetCommand { get; }
        public DelegateCommand SaveCommand { get; }


        private void ResetFields()
        {
            RegistrationDate = DateTime.Today;
            SelectedCountry = CountryDetails.FirstOrDefault(c => c.code == "IN")!;
            Balance = 0;
            FirstName = "";
            LastName = "";
            PersonId = "";
            Mobile = "";
        }

        private void Save()
        {
            if (FirstName.IsNullOrEmpty() || LastName.IsNullOrEmpty() || Mobile.IsNullOrEmpty() ||
                PersonId.IsNullOrEmpty() || string.IsNullOrWhiteSpace(PersonType) || SelectedCountry is null ||
                Balance < 0 || (!string.IsNullOrWhiteSpace(Email) && !new EmailAddressAttribute().IsValid(Email)))
            {
                MessageBox.Error("Please enter valid person details", "Incomplete data");
                return;
            }

            string imageName = null;
            if (!ImagePathTag.IsNullOrEmpty())
            {
                try
                {
                    var newImagepath = Path.Join(Services.AppPathWithoutName, "images",
                        $"{PersonId.Trim()}{Path.GetExtension(ImagePathTag)}");
                    File.Copy(ImagePathTag,newImagepath,true);
                    if (File.Exists(newImagepath)) imageName = Path.GetFileName(newImagepath);
                }
                catch (Exception)
                {
                    //ignored
                }
            }
            var person = new Person(FirstName.Trim(), LastName.Trim(), PersonType, PersonId.Trim(),
                Balance, imageName, RegistrationDate.ToShortDateString(),
                $"{SelectedCountry.dial_code}{Mobile}", email: Email);
            try
            {
                using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
                connection.Insert<Person, int>(person);
                ResetFields();
                LoadPeople();
            }
            catch (Exception e)
            {
                MessageBox.Error(e.Message);
            }

        }

        private BaseCommand addPayment;
        public ICommand AddPayment => addPayment ??= new BaseCommand(PerformAddPayment);

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
    }
}
