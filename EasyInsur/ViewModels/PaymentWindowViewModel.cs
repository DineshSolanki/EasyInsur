using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyInsur.Models;
using EasyInsur.Modules;
using HandyControl.Tools.Extension;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace EasyInsur.ViewModels
{
    public class PaymentWindowViewModel : BindableBase, IConfirmNavigationRequest
    {
        private readonly IAppDialogService _dialogService;
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }
        public PaymentWindowViewModel(IAppDialogService? dialogService = null)
        {
            _dialogService = dialogService ?? AppDialogService.Current;
            ResetCommand = new DelegateCommand(ResetFields);
            SaveCommand = new DelegateCommand(Save);
        }

        public DelegateCommand ResetCommand { get; }
        public DelegateCommand SaveCommand { get; }

        #region Properties

        private string _srNo;

        public string SrNo
        {
            get => _srNo;
            set => SetProperty(ref _srNo, value);
        }

        private DateTime _firstDate = DateTime.Now;

        public DateTime FirstDate
        {
            get => _firstDate;
            set => SetProperty(ref _firstDate, value);
        }

        private IEnumerable<Transactions> _transactions;

        public IEnumerable<Transactions> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        private double _totalAmount;
        private double _taxAmount;
        private double _tPAmount;
        private double _oDAmount;
        private double _fixedAmount;

        private double _totalCommission;

        public double TotalCommission
        {
            get => _totalCommission;
            set => SetProperty(ref _totalCommission, value);
        }

        public double TotalAmount
        {
            get => _totalAmount;
            set
            {
                SetProperty(ref _totalAmount, value);
                CalculateCommissionAmount();
            }
        }

        public double TaxAmount
        {
            get => _taxAmount;
            set
            {
                SetProperty(ref _taxAmount, value);
                CalculateInsuranceAmount();
            }
        }

        public double TPAmount
        {
            get => _tPAmount;
            set
            {
                SetProperty(ref _tPAmount, value);
                CalculateInsuranceAmount();
            }
        }

        public double ODAmount
        {
            get => _oDAmount;
            set
            {
                SetProperty(ref _oDAmount, value);
                CalculateInsuranceAmount();
            }
        }

        public double FixedAmount
        {
            get => _fixedAmount;
            set
            {
                SetProperty(ref _fixedAmount, value);
                CalculateInsuranceAmount();
            }
        }

        private double _commissionAmount;

        public double CommissionAmount
        {
            get => _commissionAmount;
            set
            {
                SetProperty(ref _commissionAmount, value);
                CalculateCommissionAmount();
            }
        }

        private double _odPercent;

        public double ODPercent
        {
            get => _odPercent;
            set
            {
                SetProperty(ref _odPercent, value);
                CalculateCommissionAmount();
            }
        }

        private double _tpPercent;

        public double TPPercent
        {
            get => _tpPercent;
            set
            {
                SetProperty(ref _tpPercent, value);
                CalculateCommissionAmount();
            }

        }

        private double _amtAfterCommission;

        public double AmtAfterCommission
        {
            get => _amtAfterCommission;
            set => SetProperty(ref _amtAfterCommission, value);
        }

        private DateTime _paymentDate = DateTime.Now;

        public DateTime PaymentDate
        {
            get => _paymentDate;
            set => SetProperty(ref _paymentDate, value);
        }

        private double _payment;

        public double Payment
        {
            get => _payment;
            set
            {
                SetProperty(ref _payment, value);
                CalculateBalance();
            }
        }

        private double _finalBalance;

        public double FinalBalance
        {
            get => _finalBalance;
            set => SetProperty(ref _finalBalance, value);
        }

        private double _previousBalance;

        public double PreviousBalance
        {
            get => _previousBalance;
            set => SetProperty(ref _previousBalance, value);
        }

        private double _balance;

        public double Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }

        private PersonType _payeeType;

        public PersonType PayeeType
        {
            get => _payeeType;
            set
            {
                SetProperty(ref _payeeType, value);
                _ = LoadPayeesAsync(value);
            }
        }

        private IEnumerable<Insurance> _insurance;

        public IEnumerable<Insurance> Insurance
        {
            get => _insurance;
            set => SetProperty(ref _insurance, value);
        }

        private string _vehicleNo;

        public string VehicleNo
        {
            get => _vehicleNo;
            set => SetProperty(ref _vehicleNo, value);
        }

        private string _vehicleRegDate;

        public string VehicleRegDate
        {
            get => _vehicleRegDate;
            set => SetProperty(ref _vehicleRegDate, value);
        }

        private string _commissionType;

        public string CommissionType
        {
            get => _commissionType;
            set
            {
                IsPercentageSelected = value == "Percentage";
                SetProperty(ref _commissionType, value);
                CalculateCommissionAmount();
                CalculateBalance();
            }
        }

        private Person _selectedPayee;

        public Person? SelectedPayee
        {
            get => _selectedPayee;
            set
            {
                SetProperty(ref _selectedPayee!, value);
                if (value is not null)
                {
                    Insurance = DbMethods.GetInsurances();
                    _ = RefreshTransactionsAsync(SelectedPayee.Id);
                    PreviousBalance = Util.RoundCurrency(DbMethods.GetBalance(SelectedPayee.Id));
                }
                else
                {
                    PreviousBalance = 0;
                }

            }
        }

        private bool _isPercentageSelected;

        public bool IsPercentageSelected
        {
            get => _isPercentageSelected;
            set => SetProperty(ref _isPercentageSelected, value);
        }

        private IEnumerable<Person> _payeeCollection;

        public IEnumerable<Person> PayeeCollection
        {
            get => _payeeCollection;
            set => SetProperty(ref _payeeCollection, value);
        }

        private IEnumerable<PersonType> _personTypes;

        public IEnumerable<PersonType> PersonTypes
        {
            get => _personTypes;
            set => SetProperty(ref _personTypes, value);
        }

        #endregion

        private void CalculateInsuranceAmount()
        {
            var total = (decimal)FixedAmount + (decimal)ODAmount + (decimal)TPAmount + (decimal)TaxAmount;
            TotalAmount = (double)Math.Round(total, 2, MidpointRounding.AwayFromZero);
            CalculateCommissionAmount();
        }

        private void CalculateCommissionAmount()
        {
            if (IsPercentageSelected)
            {
                var odComm = (decimal)Util.GetPercentageOf(ODPercent, ODAmount);
                var tpComm = (decimal)Util.GetPercentageOf(TPPercent, TPAmount);
                var totalComm = Math.Round(odComm + tpComm, 2, MidpointRounding.AwayFromZero);
                TotalCommission = (double)totalComm;
                AmtAfterCommission = (double)Math.Round((decimal)TotalAmount - totalComm, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                var totalComm = Math.Round((decimal)CommissionAmount, 2, MidpointRounding.AwayFromZero);
                TotalCommission = (double)totalComm;
                AmtAfterCommission = (double)Math.Round((decimal)TotalAmount - totalComm, 2, MidpointRounding.AwayFromZero);
            }
            CalculateBalance();
        }

        private void CalculateBalance()
        {
            var bal = Math.Round((decimal)AmtAfterCommission - (decimal)Payment, 2, MidpointRounding.AwayFromZero);
            Balance = (double)bal;
            FinalBalance = (double)Math.Round((decimal)PreviousBalance + bal, 2, MidpointRounding.AwayFromZero);
        }

        private void ResetFields()
        {
            PaymentDate = FirstDate = DateTime.Now;
            SelectedPayee = null;
            VehicleNo = "";
            FixedAmount = ODAmount = TPAmount = TaxAmount = TotalAmount = 0;
            CommissionAmount = ODPercent = TPPercent = AmtAfterCommission = 0;
            Payment = Balance = PreviousBalance = FinalBalance = 0;
            IsDirty = false;
        }

        private bool ValidatePaymentInputs()
        {
            if (SelectedPayee is null)
            {
                _dialogService.ShowError("Please select a Payee (Customer or Agent).", "Validation Error");
                return false;
            }

            if (string.IsNullOrWhiteSpace(VehicleNo))
            {
                _dialogService.ShowError("Please enter a valid Vehicle Number.", "Validation Error");
                return false;
            }

            if (FixedAmount < 0 || ODAmount < 0 || TPAmount < 0 || TaxAmount < 0)
            {
                _dialogService.ShowError("Insurance premium and tax amounts cannot be negative.", "Validation Error");
                return false;
            }

            if (TotalAmount <= 0)
            {
                _dialogService.ShowError("Total premium amount must be greater than zero.", "Validation Error");
                return false;
            }

            if (IsPercentageSelected)
            {
                if (ODPercent is < 0 or > 100 || TPPercent is < 0 or > 100)
                {
                    _dialogService.ShowError("Commission percentage must be between 0% and 100%.", "Validation Error");
                    return false;
                }
            }
            else
            {
                if (CommissionAmount < 0)
                {
                    _dialogService.ShowError("Commission amount cannot be negative.", "Validation Error");
                    return false;
                }
            }

            if (Payment < 0)
            {
                _dialogService.ShowError("Payment amount cannot be negative.", "Validation Error");
                return false;
            }

            if (PaymentDate.Date > DateTime.Today)
            {
                _dialogService.ShowError("Payment date cannot be in the future.", "Validation Error");
                return false;
            }

            return true;
        }

        private void Save()
        {
            if (!ValidatePaymentInputs())
                return;

            CalculateBalance();
            try
            {
                long? iid;
                var trimmedVehicleNo = VehicleNo.Trim();
                if (Insurance.Any() && Insurance.Any(i => string.Equals(i.VehicleNo, trimmedVehicleNo, StringComparison.OrdinalIgnoreCase)))
                    iid = Insurance.First(i => string.Equals(i.VehicleNo, trimmedVehicleNo, StringComparison.OrdinalIgnoreCase)).Id!;
                else
                {
                    DbMethods.SaveInsurance(new Insurance()
                    {
                        RegDate = FirstDate.ToShortDateString(),
                        VehicleNo = trimmedVehicleNo
                    });
                    iid = DbMethods.GetInsuranceId(trimmedVehicleNo);
                }

                if (iid is null)
                {
                    _dialogService.ShowError("Could not retrieve vehicle insurance ID. Transaction aborted.", "Database Error");
                    return;
                }

                var transaction = new Transactions()
                {
                    FixedAmount = Util.RoundCurrency(FixedAmount),
                    InsuranceID = iid,
                    OD = Util.RoundCurrency(ODAmount),
                    TP = Util.RoundCurrency(TPAmount),
                    Tax = Util.RoundCurrency(TaxAmount),
                    TotalAmount = Util.RoundCurrency(TotalAmount),
                    CommissionAmount = Util.RoundCurrency(TotalCommission),
                    Balance = Util.RoundCurrency(Balance),
                    PreviousBalance = Util.RoundCurrency(PreviousBalance),
                    PersonID = SelectedPayee.Id,
                    AfterCommissionAmount = Util.RoundCurrency(AmtAfterCommission),
                    CommissionType = CommissionType,
                    FinalBalance = Util.RoundCurrency(FinalBalance),
                    ODPercent = Util.RoundCurrency(ODPercent),
                    TPPercent = Util.RoundCurrency(TPPercent),
                    PaymentDate = PaymentDate.ToShortDateString(),
                    Payment = Util.RoundCurrency(Payment)
                };
                var id = DbMethods.SaveTransaction(transaction);
                SelectedPayee.Balance = Util.RoundCurrency(FinalBalance);
                DbMethods.UpdatePersonBalance(SelectedPayee);
                Insurance = DbMethods.GetInsurances();
                _ = RefreshTransactionsAsync(SelectedPayee.Id);
                PreviousBalance = Util.RoundCurrency(DbMethods.GetBalance(SelectedPayee.Id));

                _dialogService.NotifySuccess($"Payment transaction #{id} saved successfully!");
                IsDirty = false;
            }
            catch (Exception e)
            {
                App.LogException(e, "PaymentWindow Save");
                _dialogService.ShowError(e.Message, "Transaction Error");
            }

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var person = navigationContext.Parameters.GetValue<Person>("person");
            if (person is null) return;
            Enum.TryParse(person.Type,true,out PersonType personType);
            PayeeType = personType;
            SelectedPayee = PayeeCollection.FindFirst(p => p.Id == person.Id);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (IsDirty)
            {
                var confirmed = _dialogService.Confirm(
                    "You have unsaved payment entries. Are you sure you want to discard them and navigate away?",
                    "Unsaved Changes");
                continuationCallback(confirmed);
            }
            else
            {
                continuationCallback(true);
            }
        }

        private async Task LoadPayeesAsync(PersonType personType)
        {
            try
            {
                PayeeCollection = personType switch
                {
                    PersonType.Customer => await DbMethods.LoadCustomers(),
                    PersonType.Agent => await DbMethods.LoadAgents(),
                    _ => DbMethods.GetPeople()
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private async Task RefreshTransactionsAsync(long? personId)
        {
            try
            {
                Transactions = await DbMethods.GetTransactions(personId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
