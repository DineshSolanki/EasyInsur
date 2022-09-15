using System.Collections.Generic;
using System.Threading.Tasks;
using EasyInsur.Models;

namespace EasyInsur.Repository;

public interface IDbRepository
{
    // Create
    object CreatePerson(Person person);
    object CreateInsurance(Insurance insurance);
    object CreateTransaction(Transactions transaction);

    object CreatePersonAsync(Person person);
    object CreateInsuranceAsync(Insurance insurance);
    object CreateTransactionAsync(Transactions transactions);


    // GetAll
    IEnumerable<Person> GetPeople();

    Task<IEnumerable<Person>> GetPeopleAsync();
    IEnumerable<Insurance> GetInsurance();
    Task<IEnumerable<Insurance>> GetInsuranceAsync();

    // GetTransactions
    public IEnumerable<Transactions> GetPeopleTransactions(int personId);
    public Task<IEnumerable<Transactions>> GetPeopleTransactionsAsync(int personId);

    //Get specific
    object GetPeopleLastId();
    Task<object> GetPeopleLastIdAsync();
    IEnumerable<Person> GetAgents();
    Task<IEnumerable<Person>> GetAgentsAsync();
    
    IEnumerable<Person> GetCustomers();
    Task<IEnumerable<Person>> GetCustomersAsync();

    long GetCustomerCount();
    Task<long> GetCustomerCountAsync();
    
    long GetAgentCount();
    Task<long> GetAgentCountAsync();

    IEnumerable<Insurance> GetInsurance(long insuranceId);
    Task<IEnumerable<Insurance>> GetInsuranceAsync(long insuranceId);

    IEnumerable<Transactions> GetCustomerTransactions();
    Task<IEnumerable<Transactions>> GetCustomerTransactionsAsync();
    
    IEnumerable<Transactions> GetAgentTransactions();
    Task<IEnumerable<Transactions>> GetAgentTransactionsAsync();
    long? GetInsuranceIdByVehicleNo(string vehicleNo);
    Task<long>? GetInsuranceIdByVehicleNoAsync(string vehicleNo);
    
    //update All
    int UpdatePeople(IEnumerable<Person> people);
    Task<int> UpdatePeopleAsync(IEnumerable<Person> people);

    int UpdateTransactions(IEnumerable<Transactions> transactions);
    Task<int> UpdateTransactionsAsync(IEnumerable<Transactions> transactions);
    
    //Update specific
    int UpdatePersonBalance(Person person);
    Task<int> UpdatePersonBalanceAsync(Person person);
}