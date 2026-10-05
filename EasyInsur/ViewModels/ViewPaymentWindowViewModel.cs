using EasyInsur.Models;
using EasyInsur.Modules;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace EasyInsur.ViewModels
{
    public class ViewPaymentWindowViewModel : BindableBase
    {
        public IReadOnlyList<string> AiSuggestions { get; } = new[]
        {
            "Filter FinalBalance greaterThan 0",
            "Sort by PaymentDate descending",
            "Group by Insurance.VehicleNo"
        };

        public ViewPaymentWindowViewModel()
        {
            LoadTransactions();
            PersonType = PersonType.Any;
            ReloadCommand = new DelegateCommand(ReloadMethod);
            SaveCommand = new DelegateCommand(SaveMethod);
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
                DbMethods.GetTransactions(SelectedPayee.Id).ContinueWith(r => Transactions = r.Result);
            }
        }
        #endregion

        #region Delegates
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ReloadCommand { get; }
        #endregion

        #region Methods
        private void SaveMethod()
        {
            var r = AllTransactions.Where(p => p.EditedColumns.Any());
            var enumerable = r.ToList();
            if (!enumerable.Any()) return;
            DbMethods.UpdateTransactions(enumerable);
        }

        private void ReloadMethod()
        {
            LoadTransactions();
            PersonType = _personType;
        }

        private void LoadTransactions()
        {
            DbMethods.LoadCustomers().ContinueWith((c) =>
            {
                Customers = c.Result;
                DbMethods.LoadAgents().ContinueWith(a =>
                {
                    Agents = a.Result;
                    People = Customers.Join(Agents);
                });
            });
            DbMethods.GetCustomerTransactions().ContinueWith(ct =>
            {
                CustomerTransactions = ct.Result;
                DbMethods.GetAgentTransactions().ContinueWith(at =>
                {
                    AgentTransactions = at.Result;
                    AllTransactions = CustomerTransactions.Join(AgentTransactions);
                });
            });

        }
        #endregion
    }
}
