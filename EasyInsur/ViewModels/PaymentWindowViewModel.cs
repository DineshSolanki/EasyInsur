using System;
using System.Collections.Generic;
using System.Linq;
using EasyInsur.Models;
using EasyInsur.Modules;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace EasyInsur.ViewModels
{
    public class PaymentWindowViewModel : BindableBase, INavigationAware
    {
        public PaymentWindowViewModel()
        {
            ResetCommand = new DelegateCommand(ResetFields);
            SaveCommand = new DelegateCommand(Save);
            //CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            //ci.DateTimeFormat.LongDatePattern = "MMM/yyyy"; //This can be used for one type of DatePicker
            //ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy"; //for the second type
            //Thread.CurrentThread.CurrentCulture = ci;
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
                switch (value)
                {
                    case PersonType.Customer:
                        PayeeCollection = DbMethods.LoadCustomers().Result;
                        break;
                    case PersonType.Agent:
                        PayeeCollection = DbMethods.LoadAgents().Result;
                        break;
                    case PersonType.Any:
                    default:
                        PayeeCollection = DbMethods.GetPeople();
                        break;
                }
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
                    DbMethods.GetTransactions(SelectedPayee.Id).ContinueWith(r => Transactions = r.Result);
                    PreviousBalance = DbMethods.GetBalance(SelectedPayee.Id);
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
            TotalAmount = FixedAmount + ODAmount + TPAmount + TaxAmount;
        }

        private void CalculateCommissionAmount()
        {
            if (IsPercentageSelected)
            {
                TotalCommission = Util.GetPercentageOf(ODPercent, ODAmount) + Util.GetPercentageOf(TPPercent, TPAmount);
                AmtAfterCommission = TotalAmount - TotalCommission;
            }
            else
            {
                TotalCommission = CommissionAmount;
                AmtAfterCommission = TotalAmount - TotalCommission;
            }
        }

        private void CalculateBalance()
        {
            Balance = AmtAfterCommission - Payment;
            FinalBalance = PreviousBalance + Balance;
        }

        private void ResetFields()
        {
            PaymentDate = FirstDate = DateTime.Now;
            SelectedPayee = null;
            VehicleNo = "";
            FixedAmount = ODAmount = TPAmount = TaxAmount = TotalAmount = 0;
            CommissionAmount = ODPercent = TPPercent = AmtAfterCommission = 0;
            Payment = Balance = PreviousBalance = FinalBalance = 0;
        }

        private void Save()
        {
            if (SelectedPayee is null ||
                VehicleNo.IsNullOrEmpty())
            {
                MessageBox.Error("Please fill all required values", "Incomplete data");
                return;
            }
            CalculateBalance();
            try
            {
                long? iid;
                if (Insurance.Any() && Insurance.Any(i => i.VehicleNo == VehicleNo))
                    iid = Insurance.First(i => i.VehicleNo == VehicleNo).Id!;
                else
                {
                    DbMethods.SaveInsurance(new Insurance()
                    {
                        RegDate = FirstDate.ToShortDateString(),
                        VehicleNo = VehicleNo
                    });
                    iid = DbMethods.GetInsuranceId(VehicleNo);
                }

                if (iid is null)
                {
                    MessageBox.Error("An Error Occurred", "Transaction aborted");
                    return;
                }

                var transaction = new Transactions()
                {
                    FixedAmount = FixedAmount,
                    InsuranceID = iid,
                    OD = ODAmount,
                    TP = TPAmount,
                    Tax = TaxAmount,
                    TotalAmount = TotalAmount,
                    CommissionAmount = TotalCommission,
                    Balance = Balance,
                    PreviousBalance = PreviousBalance,
                    PersonID = SelectedPayee.Id,
                    AfterCommissionAmount = AmtAfterCommission,
                    CommissionType = CommissionType,
                    FinalBalance = FinalBalance,
                    ODPercent = ODPercent,
                    TPPercent = TPPercent,
                    PaymentDate = PaymentDate.ToShortDateString(),
                    Payment = Payment
                };
                var id = DbMethods.SaveTransaction(transaction);
                SelectedPayee.Balance = FinalBalance;
                DbMethods.UpdatePersonBalance(SelectedPayee);
                Insurance = DbMethods.GetInsurances();
                DbMethods.GetTransactions(SelectedPayee.Id).ContinueWith(r => Transactions = r.Result);
                PreviousBalance = DbMethods.GetBalance(SelectedPayee.Id);

            }
            catch (Exception e)
            {
                MessageBox.Error(e.Message);
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
    }
}
