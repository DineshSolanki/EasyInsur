using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EasyInsur.Models;
using EasyInsur.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Syncfusion.UI.Xaml.Grid;

namespace EasyInsur.ViewModels
{
    public class InsuranceWindowViewModel : BindableBase, IConfirmNavigationRequest
    {
        private readonly IAppDialogService _dialogService;
        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        public InsuranceWindowViewModel(IAppDialogService? dialogService = null)
        {
            _dialogService = dialogService ?? AppDialogService.Current;

            SaveCommand = new DelegateCommand(async () => await SaveInsuranceAsync());
            ResetCommand = new DelegateCommand(ResetFields);
            ReloadCommand = new DelegateCommand(async () => await LoadInsurancesAsync());
            DeleteInsuranceCommand = new DelegateCommand<object>(PerformDeleteInsurance);

            ResetFields();
            _ = LoadInsurancesAsync();
        }

        #region Properties

        private ObservableCollection<Insurance> _insurances = new();
        public ObservableCollection<Insurance> Insurances
        {
            get => _insurances;
            set => SetProperty(ref _insurances, value);
        }

        private string _vehicleNo = string.Empty;
        public string VehicleNo
        {
            get => _vehicleNo;
            set
            {
                if (SetProperty(ref _vehicleNo, value))
                {
                    IsDirty = true;
                }
            }
        }

        private DateTime _regDate = DateTime.Today;
        public DateTime RegDate
        {
            get => _regDate;
            set
            {
                if (SetProperty(ref _regDate, value))
                {
                    IsDirty = true;
                }
            }
        }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value))
                {
                    IsDirty = true;
                }
            }
        }

        private int _totalInsurancesCount;
        public int TotalInsurancesCount
        {
            get => _totalInsurancesCount;
            set => SetProperty(ref _totalInsurancesCount, value);
        }

        private Insurance? _selectedInsurance;
        public Insurance? SelectedInsurance
        {
            get => _selectedInsurance;
            set => SetProperty(ref _selectedInsurance, value);
        }

        #endregion

        #region Commands

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ResetCommand { get; }
        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand<object> DeleteInsuranceCommand { get; }

        #endregion

        #region Methods

        public void ResetFields()
        {
            VehicleNo = string.Empty;
            RegDate = DateTime.Today;
            Amount = 0;
            SelectedInsurance = null;
            IsDirty = false;
        }

        public async Task LoadInsurancesAsync()
        {
            try
            {
                var list = await DbMethods.GetInsurancesAsync();
                Insurances = new ObservableCollection<Insurance>(list.OrderByDescending(i => i.Id));
                TotalInsurancesCount = Insurances.Count;
            }
            catch (Exception ex)
            {
                App.LogException(ex, "InsuranceWindow Load");
            }
        }

        private async Task SaveInsuranceAsync()
        {
            var trimmedVehicle = VehicleNo.Trim();
            if (string.IsNullOrWhiteSpace(trimmedVehicle))
            {
                _dialogService.ShowError("Please enter a valid Vehicle Number.", "Validation Error");
                return;
            }

            if (RegDate.Date > DateTime.Today)
            {
                _dialogService.ShowError("Registration date cannot be in the future.", "Validation Error");
                return;
            }

            if (Amount < 0)
            {
                _dialogService.ShowError("Insured amount cannot be negative.", "Validation Error");
                return;
            }

            if (Insurances.Any(i => string.Equals(i.VehicleNo, trimmedVehicle, StringComparison.OrdinalIgnoreCase)))
            {
                _dialogService.ShowError($"Vehicle '{trimmedVehicle}' is already registered in the system.", "Duplicate Vehicle");
                return;
            }

            try
            {
                var newInsurance = new Insurance
                {
                    VehicleNo = trimmedVehicle.ToUpperInvariant(),
                    RegDate = RegDate.ToShortDateString(),
                    Amount = Util.RoundCurrency(Amount)
                };

                DbMethods.SaveInsurance(newInsurance);
                _dialogService.NotifySuccess($"Vehicle '{newInsurance.VehicleNo}' registered successfully!");
                ResetFields();
                await LoadInsurancesAsync();
            }
            catch (Exception ex)
            {
                App.LogException(ex, "InsuranceWindow Save");
                _dialogService.ShowError(ex.Message, "Database Error");
            }
        }

        private async void PerformDeleteInsurance(object commandParameter)
        {
            Insurance? target = null;
            if (commandParameter is GridRecordContextMenuInfo info && info.Record is Insurance ins)
            {
                target = ins;
            }
            else if (commandParameter is Insurance directIns)
            {
                target = directIns;
            }
            else if (SelectedInsurance != null)
            {
                target = SelectedInsurance;
            }

            if (target?.Id == null) return;

            var confirmed = _dialogService.Confirm(
                $"Are you sure you want to permanently delete vehicle insurance for '{target.VehicleNo}'?\n\nThis destructive action cannot be undone.",
                "Confirm Permanent Deletion");

            if (!confirmed) return;

            try
            {
                var success = await DbMethods.DeleteInsuranceAsync(target.Id.Value);
                if (success)
                {
                    _dialogService.NotifySuccess($"Vehicle '{target.VehicleNo}' deleted successfully.");
                    await LoadInsurancesAsync();
                }
                else
                {
                    _dialogService.ShowError("Unable to delete insurance record from the database.", "Delete Failed");
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex, "InsuranceWindow Delete");
                _dialogService.ShowError(ex.Message, "Error Deleting Insurance");
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _ = LoadInsurancesAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (IsDirty)
            {
                var confirmed = _dialogService.Confirm(
                    "You have unsaved vehicle details. Are you sure you want to discard them and navigate away?",
                    "Unsaved Changes");
                continuationCallback(confirmed);
            }
            else
            {
                continuationCallback(true);
            }
        }

        #endregion
    }
}