using EasyInsur.Models;
using EasyInsur.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EasyInsur.ViewModels
{
    public class ViewPaymentWindowViewModel : BindableBase
    {
        private readonly IAppDialogService _dialogService;

        public IReadOnlyList<string> AiSuggestions { get; } = new[]
        {
            "Filter FinalBalance greaterThan 0",
            "Sort by PaymentDate descending",
            "Group by Insurance.VehicleNo"
        };

        public ViewPaymentWindowViewModel(IAppDialogService? dialogService = null)
        {
            _dialogService = dialogService ?? AppDialogService.Current;
            PersonType = PersonType.Any;
            ReloadCommand = new DelegateCommand(async () => await ReloadMethodAsync());
            SaveCommand = new DelegateCommand(SaveMethod);
            DeleteTransactionCommand = new DelegateCommand<object>(PerformDeleteTransaction);
            _ = LoadTransactionsAsync();
        }
        #region Properties
        private IEnumerable<Person> _agents;
        private IEnumerable<Person> _customers;
        private IEnumerable<Person> _people;
        public IEnumerable<Person> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }
        public IEnumerable<Person> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }
        public IEnumerable<Person> Agents
        {
            get => _agents;
            set => SetProperty(ref _agents, value);
        }
        private IEnumerable<Person> _payeeCollection;
        public IEnumerable<Person> PayeeCollection
        {
            get => _payeeCollection;
            set => SetProperty(ref _payeeCollection, value);
        }

        private PersonType _personType;
        public PersonType PersonType
        {
            get => _personType;
            set
            {
                SetProperty(ref _personType, value);
                switch (_personType)
                {
                    case PersonType.Agent:
                        Transactions = AgentTransactions;
                        PayeeCollection = Agents;
                        break;
                    case PersonType.Customer:
                        Transactions = CustomerTransactions;
                        PayeeCollection = Customers;
                        break;
                    case PersonType.Any:
                        Transactions = AllTransactions;
                        PayeeCollection = People;
                        break;
                    default:
                        Transactions = AllTransactions;
                        PayeeCollection = People;
                        break;
                }
            }
        }
        private IEnumerable<Transactions> _allTransactions;
        public IEnumerable<Transactions> AllTransactions
        {
            get => _allTransactions;
            set
            {
                SetProperty(ref _allTransactions, value);

            }
        }
        private IEnumerable<Transactions> _transactions;
        public IEnumerable<Transactions> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }
        private IEnumerable<Transactions> _customerTransactions;
        public IEnumerable<Transactions> CustomerTransactions
        {
            get => _customerTransactions;
            set => SetProperty(ref _customerTransactions, value);
        }
        private IEnumerable<Transactions> _agentTransactions;
        public IEnumerable<Transactions> AgentTransactions
        {
            get => _agentTransactions;
            set => SetProperty(ref _agentTransactions, value);
        }
        private Person _selectedPayee;
        public Person SelectedPayee
        {
            get => _selectedPayee;
            set
            {
                SetProperty(ref _selectedPayee, value);
                if (value == null)
                {
                    PersonType = _personType;
                    return;
                }
                _ = LoadSelectedPayeeTransactionsAsync(value);
            }
        }
        #endregion

        #region Delegates
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand<object> DeleteTransactionCommand { get; }
        #endregion

        #region Methods
        private async void PerformDeleteTransaction(object commandParameter)
        {
            Transactions? targetTransaction = null;
            if (commandParameter is GridRecordContextMenuInfo info && info.Record is Transactions tx)
            {
                targetTransaction = tx;
            }
            else if (commandParameter is Transactions directTx)
            {
                targetTransaction = directTx;
            }

            if (targetTransaction?.Id == null) return;

            var confirmed = _dialogService.Confirm(
                $"Are you sure you want to permanently delete Transaction #{targetTransaction.Id} ({targetTransaction.PaymentDate})?\n\nThis destructive action cannot be undone.",
                "Confirm Permanent Deletion");

            if (!confirmed)
                return;

            try
            {
                var success = await DbMethods.DeleteTransactionAsync(targetTransaction.Id.Value);
                if (success)
                {
                    _dialogService.NotifySuccess($"Transaction #{targetTransaction.Id} deleted successfully.");
                    await ReloadMethodAsync();
                }
                else
                {
                    _dialogService.ShowError("Unable to delete the transaction from the database.", "Delete Failed");
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex, "Delete Transaction");
                _dialogService.ShowError(ex.Message, "Error Deleting Transaction");
            }
        }

        private void SaveMethod()
        {
            var r = AllTransactions.Where(p => p.EditedColumns.Any());
            var enumerable = r.ToList();
            if (!enumerable.Any()) return;
            DbMethods.UpdateTransactions(enumerable);
        }

        private async Task LoadSelectedPayeeTransactionsAsync(Person payee)
        {
            try
            {
                Transactions = await DbMethods.GetTransactions(payee.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private async Task ReloadMethodAsync()
        {
            await LoadTransactionsAsync();
            PersonType = _personType;
        }

        private async Task LoadTransactionsAsync()
        {
            try
            {
                var people = await Task.WhenAll(DbMethods.LoadCustomers(), DbMethods.LoadAgents());
                var transactions = await Task.WhenAll(DbMethods.GetCustomerTransactions(), DbMethods.GetAgentTransactions());

                Customers = people[0];
                Agents = people[1];
                People = Customers.Concat(Agents).ToList();

                CustomerTransactions = transactions[0];
                AgentTransactions = transactions[1];
                AllTransactions = CustomerTransactions.Concat(AgentTransactions).ToList();
                PersonType = _personType;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

        }
        #endregion
    }
}
