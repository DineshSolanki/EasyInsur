using System.Collections.Generic;
using System.Threading.Tasks;
using EasyInsur.Models;
using MySql.Data.MySqlClient;
using RepoDb;
using RepoDb.Enumerations;

namespace EasyInsur.Repository;

public class DbRepository : DbRepository<MySqlConnection>, IDbRepository
{
    public DbRepository() : base(Services.Settings.ConnectionString) { }


    public object CreatePerson(Person person)
    {
        return Insert(person);
    }

    public object CreateInsurance(Insurance insurance)
    {
        return Insert(insurance);
    }

    public object CreateTransaction(Transactions transaction)
    {
        return Insert(transaction);
    }

    public object CreatePersonAsync(Person person)
    {
        return InsertAsync(person);
    }

    public object CreateInsuranceAsync(Insurance insurance)
    {
        return InsertAsync(insurance);
    }

    public object CreateTransactionAsync(Transactions transactions)
    {
        return InsertAsync(transactions);
    }

    public IEnumerable<Person> GetPeople()
    {
        return QueryAll<Person>();
    }

    public Task<IEnumerable<Person>> GetPeopleAsync()
    {
        return QueryAllAsync<Person>();
    }

    public IEnumerable<Insurance> GetInsurance()
    {
        return QueryAll<Insurance>();
    }

    public Task<IEnumerable<Insurance>> GetInsuranceAsync()
    {
        return QueryAllAsync<Insurance>();
    }

    public IEnumerable<Transactions> GetPeopleTransactions(int personId)
    {
        return Query<Transactions>(t => t.PersonID == personId);
    }

    public Task<IEnumerable<Transactions>> GetPeopleTransactionsAsync(int personId)
    {
        return QueryAsync<Transactions>(t=>t.PersonID == personId);
    }

    public IEnumerable<Person> GetAgents()
    {
        return Query<Person>(p => p.Type == "Agent");
    }

    public Task<IEnumerable<Person>> GetAgentsAsync()
    {
        return QueryAsync<Person>(p => p.Type == "Agent");
    }

    public IEnumerable<Person> GetCustomers()
    {
        return Query<Person>(p => p.Type == "Customer");
    }

    public Task<IEnumerable<Person>> GetCustomersAsync()
    {
        return QueryAsync<Person>(p => p.Type == "Customer");
    }

    public IEnumerable<Insurance> GetInsurance(long insuranceId)
    {
        return Query<Insurance>(i => i.Id == insuranceId);
    }

    public Task<IEnumerable<Insurance>> GetInsuranceAsync(long insuranceId)
    {
        return QueryAsync<Insurance>(i => i.Id == insuranceId);
    }

    public long? GetInsuranceIdByVehicleNo(string vehicleNo)
    {
        var existing = Exists<Insurance>(i => i.VehicleNo == vehicleNo);
        if (!existing) return null;
        var param = new Dictionary<string, object>
        {
            { "vehicleNo", vehicleNo }
        };
        return ExecuteScalar<long>("SELECT id FROM Insurance WHERE  VehicleNo=@vehicleNo",param);
    }

    public Task<long>? GetInsuranceIdByVehicleNoAsync(string vehicleNo)
    {
        var existing = Exists<Insurance>(i => i.VehicleNo == vehicleNo);
        if (!existing) return null;
        var param = new Dictionary<string, object>
        {
            { "vehicleNo", vehicleNo }
        };
        return ExecuteScalarAsync<long>("SELECT id FROM Insurance WHERE  VehicleNo=@vehicleNo",param);
    }

    public int UpdatePersonBalance(Person person)
    {
        return Update(person, person.Id, Field.Parse<Person>(person1 => person1.Balance));
    }

    public Task<int> UpdatePersonBalanceAsync(Person person)
    {
        return UpdateAsync(person, person.Id, Field.Parse<Person>(person1 => person1.Balance));
    }

    public int UpdatePeople(IEnumerable<Person> people)
    {
        return UpdateAll(people);
    }

    public Task<int> UpdatePeopleAsync(IEnumerable<Person> people)
    {
        return UpdateAllAsync(people);
    }

    public int UpdateTransactions(IEnumerable<Transactions> transactions)
    {
        return UpdateAll(transactions);
    }

    public Task<int> UpdateTransactionsAsync(IEnumerable<Transactions> transactions)
    {
        return UpdateAllAsync(transactions);
    }

    public IEnumerable<Transactions> GetCustomerTransactions()
    {
        const string? query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Customer');";
        return ExecuteQuery<Transactions>(query);
    }

    public Task<IEnumerable<Transactions>> GetCustomerTransactionsAsync()
    {
        const string? query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Customer');";
        return ExecuteQueryAsync<Transactions>(query);
    }

    public IEnumerable<Transactions> GetAgentTransactions()
    {
        const string? query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Agent');";
        return ExecuteQuery<Transactions>(query);
    }

    public Task<IEnumerable<Transactions>> GetAgentTransactionsAsync()
    {
        const string? query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Agent');";
        return ExecuteQueryAsync<Transactions>(query);
    }

    public long GetCustomerCount()
    {
        return Count<Person>(p => p.Type == "Customer");
    }

    public Task<long> GetCustomerCountAsync()
    {
        return CountAsync<Person>(p => p.Type == "Customer");
    }

    public long GetAgentCount()
    {
        return Count<Person>(p => p.Type == "Agent");
    }

    public Task<long> GetAgentCountAsync()
    {
        return CountAsync<Person>(p => p.Type == "Agent");
    }

    public object GetPeopleLastId()
    {
        return MaxAll<Person>(p => p.Id);
    }

    public Task<object> GetPeopleLastIdAsync()
    {
        return MaxAllAsync<Person>(p => p.Id);
    }
}