using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;
using EasyInsur.Models;
using HandyControl.Controls;
using RepoDb;

namespace EasyInsur.Modules
{
    public static class DbMethods
    {
        public static Task<IEnumerable<Person>> LoadAgents()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var agents = connection.QueryAsync<Person>(p => p.Type == "Agent");
            return agents;
        }
        public static Task<IEnumerable<Person>> LoadCustomers()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var customers = connection.QueryAsync<Person>(p => p.Type == "Customer");
            return customers;
        }
        public static double GetBalance(long? id)
        {
            if (id is null) return 0;
            double person;
            var param = new Dictionary<string, object>
            {
                { "Id", id }
            };
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            try
            {
                person = connection.ExecuteScalarAsync<double>("SELECT Balance FROM Person WHERE  id=@Id", param).Result;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }

            return person;
        }

        public static Task<IEnumerable<Transactions>> GetTransactions(long? personId)
        {
            //if (personId is null) return new Task<IEnumerable<Transactions>>();

            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            Task<IEnumerable<Transactions>> transaction;
            try
            {
                transaction = connection.QueryAsync<Transactions>(t => t.PersonID == personId);
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
        public static Task<IEnumerable<Transactions>> GetAllTransactions()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            Task<IEnumerable<Transactions>> transaction = connection.QueryAllAsync<Transactions>();
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
        public static IEnumerable<Insurance> GetInsurance(long insuranceId)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.QueryAsync<Insurance>(i => i.Id == insuranceId).Result;
        }
        public static Task<IEnumerable<Insurance>> GetInsuranceAsync(long insuranceId)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.QueryAsync<Insurance>(i => i.Id == insuranceId);
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
            var param = new Dictionary<string, object>
            {
                { "vehicleNo", vehicleNo }
            };
            var id = connection.ExecuteScalarAsync<long>
                ("SELECT id FROM Insurance WHERE  VehicleNo=@vehicleNo",param).Result;
            return id;
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
        public static int UpdateTransactions(IEnumerable<Transactions> transactions)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.UpdateAllAsync(transactions).Result;
        }
        public static Task<IEnumerable<Transactions>> GetCustomerTransactions()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            Task<IEnumerable<Transactions>> transaction;
            try
            {
                const string? query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Customer');";
                transaction = connection.ExecuteQueryAsync<Transactions>(query);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            return transaction;
        }
        public static Task<IEnumerable<Transactions>> GetAgentTransactions()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            Task<IEnumerable<Transactions>> transaction;
            try
            {
                const string? query = "SELECT * FROM Transactions where Transactions.Id in (SELECT Id from Person where Person.Type = 'Agent');";
                transaction = connection.ExecuteQueryAsync<Transactions>(query);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            return transaction;
        }

        public static Task<Tuple<IEnumerable<Person>, IEnumerable<Person>>> GetCustomerAgentCount()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var customerAgent = connection.QueryMultipleAsync<Person, Person>
            (p => p.Type == "Agent",
                p1 => p1.Type == "Customer");
            return customerAgent;
        }

        public static Task<long> GetCustomerCount()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var count = connection.CountAsync<Person>(p => p.Type == "Customer");
            return count;
        }

        public static Task<long> GetAgentCount()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var count = connection.CountAsync<Person>(p => p.Type == "Agent");
            return count;
        }

        public static async Task<(long customerCount, long agentCount, double totalPremium, double totalCommission, double totalBalance, long transactionCount)> GetDashboardMetricsAsync()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            await connection.OpenAsync();

            long customerCount = 0;
            long agentCount = 0;
            double totalPremium = 0;
            double totalCommission = 0;
            double totalBalance = 0;
            long transactionCount = 0;

            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Person WHERE Type = 'Customer';", connection))
            {
                var val = await cmd.ExecuteScalarAsync();
                if (val != null && val != DBNull.Value) customerCount = Convert.ToInt64(val);
            }

            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Person WHERE Type = 'Agent';", connection))
            {
                var val = await cmd.ExecuteScalarAsync();
                if (val != null && val != DBNull.Value) agentCount = Convert.ToInt64(val);
            }

            using (var cmd = new SQLiteCommand("SELECT COALESCE(SUM(Balance), 0) FROM Person;", connection))
            {
                var val = await cmd.ExecuteScalarAsync();
                if (val != null && val != DBNull.Value) totalBalance = Convert.ToDouble(val);
            }

            using (var cmd = new SQLiteCommand("SELECT COUNT(*), COALESCE(SUM(TotalAmount), 0), COALESCE(SUM(CommissionAmount), 0) FROM Transactions;", connection))
            {
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    transactionCount = reader.GetInt64(0);
                    totalPremium = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                    totalCommission = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);
                }
            }

            return (customerCount, agentCount, totalPremium, totalCommission, totalBalance, transactionCount);
        }

        public static async Task<List<DashboardActivityItem>> GetRecentDashboardActivityAsync(int limit = 10)
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            await connection.OpenAsync();

            var list = new List<DashboardActivityItem>();
            const string query = @"
                SELECT 
                    t.Id, 
                    COALESCE(t.PaymentDate, '') as PaymentDate, 
                    COALESCE(t.TotalAmount, 0) as TotalAmount, 
                    COALESCE(t.CommissionAmount, 0) as CommissionAmount, 
                    COALESCE(t.Payment, 0) as Payment, 
                    COALESCE(t.Balance, 0) as Balance, 
                    COALESCE(p.FirstName || ' ' || p.LastName, 'Unknown') as PayeeName, 
                    COALESCE(p.Type, 'Customer') as PayeeType, 
                    COALESCE(i.VehicleNo, '-') as VehicleNo 
                FROM Transactions t 
                LEFT JOIN Person p ON t.PersonID = p.Id 
                LEFT JOIN Insurance i ON t.InsuranceID = i.Id 
                ORDER BY t.Id DESC 
                LIMIT @Limit;";

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Limit", limit);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DashboardActivityItem
                {
                    Id = reader.GetInt64(0),
                    PaymentDate = reader.GetString(1),
                    TotalAmount = reader.GetDouble(2),
                    CommissionAmount = reader.GetDouble(3),
                    Payment = reader.GetDouble(4),
                    Balance = reader.GetDouble(5),
                    PayeeName = reader.GetString(6),
                    PayeeType = reader.GetString(7),
                    VehicleNo = reader.GetString(8)
                });
            }

            return list;
        }

        public static Task<object> GetPeopleLastId()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var id = connection.MaxAllAsync<Person>(p=> p.Id);
            return id;
        }
    }
}
