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
using HandyControl.Tools.Extension;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using RepoDb;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid;

namespace EasyInsur.ViewModels
{
    public class PersonDetailsViewModel : BindableBase, IConfirmNavigationRequest
    {
        private readonly IRegionManager _regionManager;
        private readonly IAppDialogService _dialogService;
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }
        public PersonDetailsViewModel(IRegionManager regionManager, IAppDialogService? dialogService = null)
        {
            _regionManager = regionManager;
            _dialogService = dialogService ?? AppDialogService.Current;
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
            SelectedCountry = CountryDetails?.FirstOrDefault(c => c.code == "IN") ?? CountryDetails?.FirstOrDefault()!;
            Balance = 0;
            FirstName = "";
            LastName = "";
            PersonId = (DbMethods.GetPeopleLastIdSync() + 1).ToString();
            Mobile = "";
            Email = "";
            ImagePathTag = "";
            IsDirty = false;
        }

        private bool ValidatePersonInputs()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                _dialogService.ShowError("Please enter First Name.", "Validation Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                _dialogService.ShowError("Please enter Last Name.", "Validation Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(PersonType))
            {
                _dialogService.ShowError("Please select Person Type (Customer or Agent).", "Validation Error");
                return false;
            }

            if (SelectedCountry == null)
            {
                _dialogService.ShowError("Please select a Country.", "Validation Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Mobile) || Mobile.Trim().Length < 5)
            {
                _dialogService.ShowError("Please enter a valid Mobile number.", "Validation Error");
                return false;
            }

            try
            {
                var fullNumber = $"{SelectedCountry.dial_code}{Mobile.Trim()}";
                var parsedNumber = Services.PhoneNumberUtil.Parse(fullNumber, SelectedCountry.code);
                if (!Services.PhoneNumberUtil.IsValidNumber(parsedNumber))
                {
                    _dialogService.ShowError($"Mobile number '{Mobile.Trim()}' is not valid for {SelectedCountry.name}.", "Validation Error");
                    return false;
                }
            }
            catch
            {
                if (!Mobile.Trim().All(char.IsDigit) || Mobile.Trim().Length < 7)
                {
                    _dialogService.ShowError("Please enter a valid numeric mobile number.", "Validation Error");
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(Email) && !new EmailAddressAttribute().IsValid(Email.Trim()))
            {
                _dialogService.ShowError("Please enter a valid Email address.", "Validation Error");
                return false;
            }

            if (Balance < 0)
            {
                _dialogService.ShowError("Opening balance cannot be negative.", "Validation Error");
                return false;
            }

            return true;
        }

        private void Save()
        {
            if (!ValidatePersonInputs())
                return;

            string? imageName = null;
            if (!ImagePathTag.IsNullOrEmpty())
            {
                try
                {
                    var imagesDir = Path.Join(Services.AppPathWithoutName, "images");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }
                    var newImagepath = Path.Join(imagesDir, $"{PersonId.Trim()}{Path.GetExtension(ImagePathTag)}");
                    File.Copy(ImagePathTag, newImagepath, true);
                    if (File.Exists(newImagepath)) imageName = Path.GetFileName(newImagepath);
                }
                catch (Exception ex)
                {
                    App.LogException(ex, "PersonDetails Image Copy");
                }
            }

            var person = new Person(
                FirstName.Trim(),
                LastName.Trim(),
                PersonType,
                PersonId.Trim(),
                Util.RoundCurrency(Balance),
                imageName,
                RegistrationDate.ToShortDateString(),
                $"{SelectedCountry.dial_code}{Mobile.Trim()}",
                email: string.IsNullOrWhiteSpace(Email) ? null : Email.Trim());

            try
            {
                using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
                connection.Insert<Person, int>(person);
                _dialogService.NotifySuccess($"Person '{FirstName.Trim()} {LastName.Trim()}' added successfully!");
                IsDirty = false;
                ResetFields();
                LoadPeople();
            }
            catch (Exception e)
            {
                App.LogException(e, "PersonDetails Save");
                _dialogService.ShowError(e.Message, "Database Error");
            }

        }

        public void OnNavigatedTo(NavigationContext navigationContext) { }
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (IsDirty)
            {
                var confirmed = _dialogService.Confirm(
                    "You have unsaved person details. Are you sure you want to discard them and navigate away?",
                    "Unsaved Changes");
                continuationCallback(confirmed);
            }
            else
            {
                continuationCallback(true);
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
