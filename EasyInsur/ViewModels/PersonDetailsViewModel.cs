using System;
using System.Collections.Generic;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using HandyControlWpfCoreApp1.Models;
using HandyControlWpfCoreApp1.Modules;
using Prism.Commands;
using RepoDb;

namespace HandyControlWpfCoreApp1.ViewModels
{
    public class PersonDetailsViewModel : BindableBase
    {
        public PersonDetailsViewModel()
        {
            CountryDetails = new ObservableCollection<Country>(Util.Read()!.OrderBy(c => c.code));
            ResetCommand = new DelegateCommand(ResetFields);
            SaveCommand = new DelegateCommand(Save);
            LoadPeople();
            ResetFields();
        }

        private void LoadPeople()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var people = connection.QueryAll<Person>();
            PersonData = people;
        }

        public IEnumerable<Person> _people;
        public IEnumerable<Person> PersonData { get => _people; set => SetProperty(ref _people,value); }
        private ObservableCollection<Country> _countryDetails;

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
                PersonData = PersonType == "Agent" ? DBMethods.LoadAgents() : DBMethods.LoadCustomers();
            }
        }

        private string _personID;
        public string PersonID
        {
            get => _personID;
            set => SetProperty(ref _personID, value);
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

        private string _registrationDate;
        public string RegistrationDate
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

        private string _imagePath = "";
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
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
            RegistrationDate = DateTime.Now.ToShortDateString();
            SelectedCountry = CountryDetails.FirstOrDefault(c => c.code == "IN")!;
            Balance = 0;
            FirstName = "";
            LastName = "";
            PersonID = "";
            Mobile = "";
        }

        private void Save()
        {
            if (FirstName.IsNullOrEmpty() || LastName.IsNullOrEmpty() || Mobile.IsNullOrEmpty() || PersonID.IsNullOrEmpty() | RegistrationDate.IsNullOrEmpty())
            {
                MessageBox.Error("Please fill all required values", "Incomplete data");
                return;
            }

            var person = new Person(FirstName.Trim(), LastName.Trim(), PersonType, PersonID.Trim(),
                Balance, ImagePath.Trim(), RegistrationDate.Trim(),
                $"{SelectedCountry.dial_code}{Mobile}");
            try
            {
                using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
                connection.Insert<Person,int>(person);
                ResetFields();
                LoadPeople();
            }
            catch (Exception e)
            {
                MessageBox.Error(e.Message);
            }

        }

    }
}
