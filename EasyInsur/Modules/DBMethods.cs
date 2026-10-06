using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using EasyInsur.Models;
using HandyControl.Controls;
using RepoDb;

namespace EasyInsur.Modules
{
    public static class DbMethods
    {
        public static async Task<List<Person>> LoadAgents()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return (await connection.QueryAsync<Person>(p => p.Type == "Agent")).ToList();
        }
        public static async Task<List<Person>> LoadCustomers()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return (await connection.QueryAsync<Person>(p => p.Type == "Customer")).ToList();
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
                connection.Open();
                using var command = new SQLiteCommand("SELECT Balance FROM Person WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id.Value);
                person = Convert.ToDouble(command.ExecuteScalar() ?? 0d);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }

            return person;
        }

        public static async Task<List<Transactions>> GetTransactions(long? personId)
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var transactions = (await connection.QueryAsync<Transactions>(t => t.PersonID == personId)).ToList();
            await AttachInsuranceAsync(transactions);
            return transactions;
        }

        public static object SaveTransaction(Transactions transactions)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.Insert(transactions);
        }
        public static async Task<List<Transactions>> GetAllTransactions()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var transactions = (await connection.QueryAllAsync<Transactions>()).ToList();
            await AttachInsuranceAsync(transactions);
            return transactions;
        }

        public static IEnumerable<Person> GetPeople()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.QueryAll<Person>().ToList();
        }
        public static IEnumerable<Insurance> GetInsurances()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.QueryAll<Insurance>().ToList();
        }
        public static IEnumerable<Insurance> GetInsurance(long insuranceId)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return connection.Query<Insurance>(i => i.Id == insuranceId).ToList();
        }
        public static async Task<List<Insurance>> GetInsuranceAsync(long insuranceId)
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return (await connection.QueryAsync<Insurance>(i => i.Id == insuranceId)).ToList();
        }

        private static async Task AttachInsuranceAsync(IEnumerable<Transactions> transactions)
        {
            var insurances = (await GetInsurancesAsync())
                .Where(i => i.Id.HasValue)
                .ToDictionary(i => i.Id!.Value);
            foreach (var transaction in transactions)
            {
                if (transaction.InsuranceID is { } insuranceId && insurances.TryGetValue(insuranceId, out var insurance))
                    transaction.Insurance = insurance;
            }
        }

        public static async Task<List<Insurance>> GetInsurancesAsync()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return (await connection.QueryAllAsync<Insurance>()).ToList();
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
            connection.Open();
            using var command = new SQLiteCommand("SELECT id FROM Insurance WHERE VehicleNo = @vehicleNo", connection);
            command.Parameters.AddWithValue("@vehicleNo", vehicleNo);
            var id = command.ExecuteScalar();
            return id is null or DBNull ? null : Convert.ToInt64(id);
        }
        public static int UpdatePersonBalance(Person person)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            connection.Open();
            using var command = new SQLiteCommand("UPDATE Person SET Balance = @Balance WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Balance", person.Balance);
            command.Parameters.AddWithValue("@Id", person.Id);
            return command.ExecuteNonQuery();
        }
        public static int UpdatePerson(Person person)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var fields = new Dictionary<string, object?>
            {
                ["FirstName"] = person.FirstName, ["LastName"] = person.LastName,
                ["Type"] = person.Type, ["PersonID"] = person.PersonID,
                ["Address"] = person.Address, ["Email"] = person.Email,
                ["Balance"] = person.Balance, ["ImagePath"] = person.ImagePath,
                ["Mobile"] = person.Mobile, ["RegDate"] = person.RegDate
            };
            var edited = person.EditedColumns
                .Where(fields.ContainsKey)
                .Distinct()
                .ToList();
            if (person.Id is null || edited.Count == 0) return 0;
            using var command = new SQLiteCommand(connection);
            command.CommandText = $"UPDATE Person SET {string.Join(", ", edited.Select(name => $"{name} = @{name}"))} WHERE Id = @Id";
            foreach (var name in edited)
                command.Parameters.AddWithValue($"@{name}", fields[name] ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", person.Id.Value);
            connection.Open();
            return command.ExecuteNonQuery();
        }
        public static int UpdatePeople(IEnumerable<Person> people)
        {
            return people.Sum(UpdatePerson);
        }
        public static int UpdateTransactions(IEnumerable<Transactions> transactions)
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            connection.Open();
            const string query = @"UPDATE Transactions SET InsuranceID=@InsuranceID, PersonID=@PersonID,
                FixedAmount=@FixedAmount, OD=@OD, TP=@TP, Tax=@Tax, TotalAmount=@TotalAmount,
                CommissionType=@CommissionType, CommissionAmount=@CommissionAmount, ODPercent=@ODPercent,
                TPPercent=@TPPercent, AfterCommissionAmount=@AfterCommissionAmount, Payment=@Payment,
                PaymentDate=@PaymentDate, Balance=@Balance, PreviousBalance=@PreviousBalance,
                FinalBalance=@FinalBalance WHERE Id=@Id";
            var count = 0;
            foreach (var transaction in transactions.Where(t => t.Id.HasValue))
            {
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@InsuranceID", transaction.InsuranceID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PersonID", transaction.PersonID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FixedAmount", transaction.FixedAmount);
                command.Parameters.AddWithValue("@OD", transaction.OD);
                command.Parameters.AddWithValue("@TP", transaction.TP);
                command.Parameters.AddWithValue("@Tax", transaction.Tax);
                command.Parameters.AddWithValue("@TotalAmount", transaction.TotalAmount);
                command.Parameters.AddWithValue("@CommissionType", transaction.CommissionType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CommissionAmount", transaction.CommissionAmount);
                command.Parameters.AddWithValue("@ODPercent", transaction.ODPercent);
                command.Parameters.AddWithValue("@TPPercent", transaction.TPPercent);
                command.Parameters.AddWithValue("@AfterCommissionAmount", transaction.AfterCommissionAmount);
                command.Parameters.AddWithValue("@Payment", transaction.Payment);
                command.Parameters.AddWithValue("@PaymentDate", transaction.PaymentDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Balance", transaction.Balance);
                command.Parameters.AddWithValue("@PreviousBalance", transaction.PreviousBalance);
                command.Parameters.AddWithValue("@FinalBalance", transaction.FinalBalance);
                command.Parameters.AddWithValue("@Id", transaction.Id!.Value);
                count += command.ExecuteNonQuery();
            }
            return count;
        }
        public static async Task<List<Transactions>> GetCustomerTransactions()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            const string query = "SELECT t.* FROM Transactions t INNER JOIN Person p ON t.PersonID = p.Id WHERE p.Type = @Type;";
            var transactions = (await connection.ExecuteQueryAsync<Transactions>(query, new { Type = "Customer" })).ToList();
            await AttachInsuranceAsync(transactions);
            return transactions;
        }
        public static async Task<List<Transactions>> GetAgentTransactions()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            const string query = "SELECT t.* FROM Transactions t INNER JOIN Person p ON t.PersonID = p.Id WHERE p.Type = @Type;";
            var transactions = (await connection.ExecuteQueryAsync<Transactions>(query, new { Type = "Agent" })).ToList();
            await AttachInsuranceAsync(transactions);
            return transactions;
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

        public static async Task<object> GetPeopleLastId()
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            return await connection.MaxAllAsync<Person>(p => p.Id);
        }

        public static long GetPeopleLastIdSync()
        {
            using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            connection.Open();
            using var command = new SQLiteCommand("SELECT COALESCE(MAX(Id), 0) FROM Person;", connection);
            return Convert.ToInt64(command.ExecuteScalar());
        }

        public static async Task<bool> DeleteTransactionAsync(long id)
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            var deletedCount = await connection.DeleteAsync<Transactions>(t => t.Id == id);
            return deletedCount > 0;
        }

        public static async Task<bool> DeletePersonAsync(long id)
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            await connection.OpenAsync();
            using (var pragmaCmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
            {
                await pragmaCmd.ExecuteNonQueryAsync();
            }
            await connection.DeleteAsync<Transactions>(t => t.PersonID == id);
            var deletedCount = await connection.DeleteAsync<Person>(p => p.Id == id);
            return deletedCount > 0;
        }

        public static async Task<bool> DeleteInsuranceAsync(long id)
        {
            await using var connection = new SQLiteConnection(Services.Settings.ConnectionString);
            await connection.OpenAsync();
            using (var pragmaCmd = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
            {
                await pragmaCmd.ExecuteNonQueryAsync();
            }
            using (var updateCmd = new SQLiteCommand("UPDATE Transactions SET InsuranceID = NULL WHERE InsuranceID = @id;", connection))
            {
                updateCmd.Parameters.AddWithValue("@id", id);
                await updateCmd.ExecuteNonQueryAsync();
            }
            var deletedCount = await connection.DeleteAsync<Insurance>(i => i.Id == id);
            return deletedCount > 0;
        }
    }
}
