using System.Data.SQLite;

var databasePath = args.Length == 1
    ? Path.GetFullPath(args[0])
    : throw new ArgumentException("Pass the path to EasyInsur.db.");

if (!File.Exists(databasePath))
    throw new FileNotFoundException("Database was not found.", databasePath);

var connectionString = $"Data Source={databasePath};Version=3;Foreign Keys=True;";
using var connection = new SQLiteConnection(connectionString);
connection.Open();
using var transaction = connection.BeginTransaction();

using (var delete = connection.CreateCommand())
{
    delete.Transaction = transaction;
    delete.CommandText = """
        DELETE FROM Transactions WHERE PersonID IN (SELECT Id FROM Person WHERE PersonID LIKE 'TEST-%');
        DELETE FROM Insurance WHERE VehicleNo LIKE 'TEST-%';
        DELETE FROM Person WHERE PersonID LIKE 'TEST-%';
        """;
    delete.ExecuteNonQuery();
}

var people = new List<(long Id, string PersonId)>();
for (var i = 1; i <= 24; i++)
{
    var type = i % 2 == 0 ? "Customer" : "Agent";
    var personId = $"TEST-{type[..1]}-{i:000}";
    using var command = connection.CreateCommand();
    command.Transaction = transaction;
    command.CommandText = """
        INSERT INTO Person
            (Type, PersonID, Address, Email, FirstName, LastName, Balance, ImagePath, Mobile, RegDate)
        VALUES
            ($type, $personId, $address, $email, $firstName, $lastName, $balance, $imagePath, $mobile, $regDate);
        SELECT last_insert_rowid();
        """;
    command.Parameters.AddWithValue("$type", type);
    command.Parameters.AddWithValue("$personId", personId);
    command.Parameters.AddWithValue("$address", $"Test address {i}");
    command.Parameters.AddWithValue("$email", $"test{i:00}@easyinsur.invalid");
    command.Parameters.AddWithValue("$firstName", "Test");
    command.Parameters.AddWithValue("$lastName", $"{type}{i:00}");
    command.Parameters.AddWithValue("$balance", i % 3 == 0 ? -125.50 : i * 125.75);
    command.Parameters.AddWithValue("$imagePath", "images/no_image.jpg");
    command.Parameters.AddWithValue("$mobile", $"900000{i:04}");
    command.Parameters.AddWithValue("$regDate", DateTime.Today.AddDays(-i).ToString("yyyy-MM-dd"));
    var id = Convert.ToInt64(command.ExecuteScalar());
    people.Add((id, personId));
}

for (var i = 1; i <= 30; i++)
{
    var person = people[(i - 1) % people.Count];
    var vehicleNo = $"TEST-{i:000}";
    using var insurance = connection.CreateCommand();
    insurance.Transaction = transaction;
    insurance.CommandText = """
        INSERT INTO Insurance (RegDate, VehicleNo, Amount)
        VALUES ($regDate, $vehicleNo, $amount);
        SELECT last_insert_rowid();
        """;
    insurance.Parameters.AddWithValue("$regDate", DateTime.Today.AddDays(-i).ToString("yyyy-MM-dd"));
    insurance.Parameters.AddWithValue("$vehicleNo", vehicleNo);
    insurance.Parameters.AddWithValue("$amount", 2500 + i * 175.25);
    var insuranceId = Convert.ToInt64(insurance.ExecuteScalar());

    var fixedAmount = 1200 + i * 25.50;
    var od = 300 + i * 7.25;
    var tp = 450 + i * 8.75;
    var tax = 18 + i * 1.10;
    var total = fixedAmount + od + tp + tax;
    var commission = i % 2 == 0 ? total * 0.05 : 125 + i;
    var payment = i % 4 == 0 ? total / 2 : total + (i % 5 == 0 ? 50 : 0);
    var balance = total - commission;
    var finalBalance = balance - payment;

    using var transactionCommand = connection.CreateCommand();
    transactionCommand.Transaction = transaction;
    transactionCommand.CommandText = """
        INSERT INTO Transactions
            (InsuranceID, PersonID, FixedAmount, OD, TP, Tax, TotalAmount,
             CommissionType, CommissionAmount, ODPercent, TPPercent,
             AfterCommissionAmount, Payment, PaymentDate, Balance,
             PreviousBalance, FinalBalance)
        VALUES
            ($insuranceId, $personId, $fixedAmount, $od, $tp, $tax, $total,
             $commissionType, $commissionAmount, $odPercent, $tpPercent,
             $afterCommissionAmount, $payment, $paymentDate, $balance,
             $previousBalance, $finalBalance);
        """;
    transactionCommand.Parameters.AddWithValue("$insuranceId", insuranceId);
    transactionCommand.Parameters.AddWithValue("$personId", person.Id);
    transactionCommand.Parameters.AddWithValue("$fixedAmount", fixedAmount);
    transactionCommand.Parameters.AddWithValue("$od", od);
    transactionCommand.Parameters.AddWithValue("$tp", tp);
    transactionCommand.Parameters.AddWithValue("$tax", tax);
    transactionCommand.Parameters.AddWithValue("$total", total);
    transactionCommand.Parameters.AddWithValue("$commissionType", i % 2 == 0 ? "Percentage" : "Fixed");
    transactionCommand.Parameters.AddWithValue("$commissionAmount", commission);
    transactionCommand.Parameters.AddWithValue("$odPercent", i % 2 == 0 ? 5 : 0);
    transactionCommand.Parameters.AddWithValue("$tpPercent", i % 2 == 0 ? 2.5 : 0);
    transactionCommand.Parameters.AddWithValue("$afterCommissionAmount", total - commission);
    transactionCommand.Parameters.AddWithValue("$payment", payment);
    transactionCommand.Parameters.AddWithValue("$paymentDate", DateTime.Today.AddDays(-i).ToString("yyyy-MM-dd"));
    transactionCommand.Parameters.AddWithValue("$balance", balance);
    transactionCommand.Parameters.AddWithValue("$previousBalance", i == 1 ? 0 : i * 100.0);
    transactionCommand.Parameters.AddWithValue("$finalBalance", finalBalance);
    transactionCommand.ExecuteNonQuery();
}

transaction.Commit();
Console.WriteLine($"Seeded 24 people, 30 insurance records, and 30 transactions into {databasePath}");
