using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using EasyInsur.Models;
using RepoDb;

namespace EasyInsur.Modules
{
    public static class DBMethods
    {
        public static IEnumerable<Person> LoadAgents()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var agents = connection.QueryAsync<Person>(p => p.Type == "Agent");
            return agents.Result;
        }
        public static IEnumerable<Person> LoadCustomers()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var customers = connection.QueryAsync<Person>(p => p.Type == "Customer");
            return customers.Result;
        }
        public static double GetBalance(long? id)
        {
            if (id is null) return 0;
            IEnumerable<Person>? person;
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            try
            {
                person = connection.QueryAsync<Person>(p => p.Id == id).Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }

            return person.FirstOrDefault().Balance;
        }

        public static IEnumerable<Transactions> GetTransactions(long? personId)
        {
            if (personId is null) return new List<Transactions>();

            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            IEnumerable<Transactions> transaction;
            try
            {
                transaction = connection.QueryAsync<Transactions>(t => t.PersonID == personId).Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            return transaction;
        }

        public static object SaveTransaction(Transactions transactions)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.Insert(transactions);
        }
        public static IEnumerable<Transactions> GetAllTransactions()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            IEnumerable<Transactions> transaction;
            transaction = connection.QueryAllAsync<Transactions>().Result;
            return transaction;
        }

        public static IEnumerable<Person> GetPeople()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.QueryAllAsync<Person>().Result;
        }
        public static IEnumerable<Insurance> GetInsurances()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.QueryAllAsync<Insurance>().Result;
        }
        public static IEnumerable<Insurance> GetInsurance(long insuranceID)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.QueryAsync<Insurance>(i => i.Id == insuranceID).Result;
        }
        public static object SaveInsurance(Insurance insurance)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var r = connection.Insert(insurance);
            return r;
        }

        public static long? GetInsuranceId(string vehicleNo)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var existing = connection.Exists<Insurance>(i =>
                i.VehicleNo == vehicleNo);
            if (!existing) return null;
            var id = connection.Query<Insurance>
                (i => i.VehicleNo == vehicleNo);
            return id.FirstOrDefault().Id;
        }
        public static int UpdatePersonBalance(Person person)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.UpdateAsync(person, person.Id, Field.Parse<Person>(person1 => person1.Balance)).Result;
        }
        public static int UpdatePerson(Person person)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var field = new List<Field>();
            person.EditedColumns.ForEach(s =>
                field.Add(new Field(s)));
            return connection.UpdateAsync(person, person.Id, field).Result;
        }
        public static int UpdatePeople(IEnumerable<Person> people)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.UpdateAllAsync(people).Result;
        }
        public static IEnumerable<Transactions> GetCustomerTransactions()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            IEnumerable<Transactions> transaction;
            try
            {
                var query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Customer');";
                transaction = connection.ExecuteQueryAsync<Transactions>(query).Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            return transaction;
        }
        public static IEnumerable<Transactions> GetAgentTransactions()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            IEnumerable<Transactions> transaction;
            try
            {
                var query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Agent');";
                transaction = connection.ExecuteQueryAsync<Transactions>(query).Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            return transaction;
        }
    }
}
