using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EasyInsur.Models;
using EasyInsur.Modules;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Prism.Commands;
using Prism.Mvvm;

namespace EasyInsur.ViewModels
{
    public class PaymentWindowViewModel : BindableBase
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
        private string _srNo;
        public string SrNo
        {
            get => _srNo;
            set => SetProperty(ref _srNo, value);
        }

        private string _firstDate = DateTime.Now.ToShortDateString();
        public string FirstDate
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

        private string _paymentDate = DateTime.Now.ToShortDateString();
        public string PaymentDate
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

        private ObservableCollection<Person> agents;
        private ObservableCollection<Person> customers;
        private string _payeeType;
        public string PayeeType
        {
            get => _payeeType;
            set
            {
                PayeeCollection = value == "Customer" ? DBMethods.LoadCustomers().ToObservableCollection() : DBMethods.LoadAgents().ToObservableCollection();
                SetProperty(ref _payeeType, value);
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
        public Person SelectedPayee
        {
            get => _selectedPayee;
            set
            {
                SetProperty(ref _selectedPayee, value);
                if (value is not null)
                {
                    Insurance = DBMethods.GetInsurances();
                    Transactions = DBMethods.GetTransactions(SelectedPayee.Id);
                    PreviousBalance = DBMethods.GetBalance(SelectedPayee.Id);
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

        private ObservableCollection<Person> _payeeCollection;
        public ObservableCollection<Person> PayeeCollection
        {
            get => _payeeCollection;
            set => SetProperty(ref _payeeCollection, value);
        }

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
            PaymentDate = FirstDate = DateTime.Now.ToShortDateString();
            SelectedPayee = null;
            VehicleNo = "";
            FixedAmount = ODAmount = TPAmount = TaxAmount = TotalAmount = 0;
            CommissionAmount = ODPercent = TPPercent = AmtAfterCommission = 0;
            Payment = Balance = PreviousBalance = FinalBalance = 0;
        }

        private void Save()
        {
            if (FirstDate.IsNullOrEmpty() ||
                PaymentDate.IsNullOrEmpty() ||
                SelectedPayee is null ||
                VehicleNo.IsNullOrEmpty() ||
                TotalAmount == 0 || Payment == 0)
            {
                MessageBox.Error("Please fill all required values", "Incomplete data");
                return;
            }
            try
            {
                long? iid;
                if (Insurance.Any() && Insurance.Any(i => i.VehicleNo == VehicleNo))
                    iid = Insurance.First(i => i.VehicleNo == VehicleNo).Id!;
                else
                {
                    DBMethods.SaveInsurance(new Insurance()
                    {
                        RegDate = FirstDate,
                        VehicleNo = VehicleNo
                    });
                    iid = DBMethods.GetInsuranceId(VehicleNo);
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
                    PaymentDate = PaymentDate,
                    Payment = Payment
                };
                var id = DBMethods.SaveTransaction(transaction);
                SelectedPayee.Balance = FinalBalance;
                DBMethods.UpdatePersonBalance(SelectedPayee);
                Insurance = DBMethods.GetInsurances();
                Transactions = DBMethods.GetTransactions(SelectedPayee.Id);
                PreviousBalance = DBMethods.GetBalance(SelectedPayee.Id);
                
            }
            catch (Exception e)
            {
                MessageBox.Error(e.Message);
            }

        }
    }
}
