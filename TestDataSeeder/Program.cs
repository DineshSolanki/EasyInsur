using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

var databasePath = args.Length >= 1
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(@"EasyInsur\EasyInsur.db");

if (!File.Exists(databasePath))
    throw new FileNotFoundException("Database was not found.", databasePath);

Console.WriteLine($"Seeding synthetic, legally-safe production-style data into: {databasePath}");

var connectionString = $"Data Source={databasePath};Version=3;Foreign Keys=True;";
using var connection = new SQLiteConnection(connectionString);
connection.Open();

using var transaction = connection.BeginTransaction();

// Clean existing data completely
using (var cleanCmd = connection.CreateCommand())
{
    cleanCmd.Transaction = transaction;
    cleanCmd.CommandText = """
        DELETE FROM Transactions;
        DELETE FROM Insurance;
        DELETE FROM Person;
        DELETE FROM sqlite_sequence WHERE name IN ('Person', 'Insurance', 'Transactions');
        """;
    cleanCmd.ExecuteNonQuery();
}

// ------------------------------------------------------------------------------------------------
// 1. SYNTHETIC PERSONS (CUSTOMERS & AGENTS)
// - Fictional names
// - Safe RFC 2606 reserved domain (@example.com / @easyinsur-demo.com) to prevent real email routing
// - Synthetic non-routing dummy phone numbers (+91-99000-01xxx)
// - Fictional sample residential and office addresses
// ------------------------------------------------------------------------------------------------
var syntheticPeople = new[]
{
    // 20 Fictional Customer Personas
    new { Type = "Customer", PersonId = "CUST-1001", First = "Aarav", Last = "Sharma", Phone = "+919900001001", Email = "aarav.sharma@example.com", Addr = "Flat 101, Lotus Blossom Residency, Grand Trunk Road, Pune, Maharashtra 411001", Reg = "2024-02-14", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1002", First = "Diya", Last = "Patel", Phone = "+919900001002", Email = "diya.patel@example.com", Addr = "Plot 24, Green Valley Enclave, Central Avenue, Gandhinagar, Gujarat 382001", Reg = "2024-03-01", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1003", First = "Vihaan", Last = "Verma", Phone = "+919900001003", Email = "vihaan.verma@example.com", Addr = "Apartment 302, Silver Heights, Outer Ring Road, Bengaluru, Karnataka 560001", Reg = "2024-03-18", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1004", First = "Ananya", Last = "Nair", Phone = "+919900001004", Email = "ananya.nair@example.com", Addr = "House 15, Pine View Colony, Mall Road, New Delhi, Delhi 110001", Reg = "2024-04-05", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1005", First = "Ishaan", Last = "Malhotra", Phone = "+919900001005", Email = "ishaan.malhotra@example.com", Addr = "Flat 504, Emerald Towers, Linking Road, Mumbai, Maharashtra 400001", Reg = "2024-04-22", Balance = 1250.0 },
    new { Type = "Customer", PersonId = "CUST-1006", First = "Aditi", Last = "Rao", Phone = "+919900001006", Email = "aditi.rao@example.com", Addr = "Plot 88, Sunrise Gardens, Canal Road, Lucknow, Uttar Pradesh 226001", Reg = "2024-05-10", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1007", First = "Reyansh", Last = "Kulkarni", Phone = "+919900001007", Email = "reyansh.kulkarni@example.com", Addr = "Villa 12, Palm Grove Estate, Lakeview Road, Bengaluru, Karnataka 560001", Reg = "2024-05-29", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1008", First = "Tara", Last = "Agarwal", Phone = "+919900001008", Email = "tara.agarwal@example.com", Addr = "Suite 201, Diamond Plaza, City Center, Ahmedabad, Gujarat 380001", Reg = "2024-06-15", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1009", First = "Kabir", Last = "Joshi", Phone = "+919900001009", Email = "kabir.joshi@example.com", Addr = "House 42, Heritage Enclave, Palace Road, Jaipur, Rajasthan 302001", Reg = "2024-07-02", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1010", First = "Riya", Last = "Gupta", Phone = "+919900001010", Email = "riya.gupta@example.com", Addr = "Flat 4B, Hillcrest Apartments, Valley View, Hyderabad, Telangana 500001", Reg = "2024-07-20", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1011", First = "Advait", Last = "Deshmukh", Phone = "+919900001011", Email = "advait.deshmukh@example.com", Addr = "Flat 203, Orchid Heights, Station Road, Pune, Maharashtra 411001", Reg = "2024-08-11", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1012", First = "Meera", Last = "Bansal", Phone = "+919900001012", Email = "meera.bansal@example.com", Addr = "Apartment 14, Royal Palms, Airport Road, New Delhi, Delhi 110001", Reg = "2024-08-28", Balance = 2450.0 },
    new { Type = "Customer", PersonId = "CUST-1013", First = "Dhruv", Last = "Tiwari", Phone = "+919900001013", Email = "dhruv.tiwari@example.com", Addr = "House 66, Crystal Enclave, Expressway Sector, Noida, Uttar Pradesh 201301", Reg = "2024-09-14", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1014", First = "Saanvi", Last = "Reddy", Phone = "+919900001014", Email = "saanvi.reddy@example.com", Addr = "Penthouse 9, Cyber Park View, High-Tech Zone, Hyderabad, Telangana 500001", Reg = "2024-10-02", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1015", First = "Arjun", Last = "Singhania", Phone = "+919900001015", Email = "arjun.singhania@example.com", Addr = "Floor 18, Horizon Tower, Marine Drive, Mumbai, Maharashtra 400001", Reg = "2024-10-19", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1016", First = "Kavya", Last = "Choudhary", Phone = "+919900001016", Email = "kavya.choudhary@example.com", Addr = "Plot 51, Golden Crest, Fortress Road, Jaipur, Rajasthan 302001", Reg = "2024-11-05", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1017", First = "Dev", Last = "Saxena", Phone = "+919900001017", Email = "dev.saxena@example.com", Addr = "Tower 3, Flat 602, Metro City Homes, Sector 45, Noida, Uttar Pradesh 201301", Reg = "2024-11-22", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1018", First = "Anika", Last = "Pillai", Phone = "+919900001018", Email = "anika.pillai@example.com", Addr = "House 77, Marina Breeze, Coast Road, Chennai, Tamil Nadu 600001", Reg = "2024-12-10", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1019", First = "Shaurya", Last = "Batra", Phone = "+919900001019", Email = "shaurya.batra@example.com", Addr = "Villa 8, Garden View Enclave, Capitol Boulevard, Chandigarh 160001", Reg = "2024-12-28", Balance = 0.0 },
    new { Type = "Customer", PersonId = "CUST-1020", First = "Pari", Last = "Sen", Phone = "+919900001020", Email = "pari.sen@example.com", Addr = "Apartment 5B, Riverdale Heights, Strand Road, Kolkata, West Bengal 700001", Reg = "2025-01-15", Balance = 0.0 },

    // 10 Fictional Licensed Agent Personas
    new { Type = "Agent", PersonId = "AGT-2001", First = "Rohan", Last = "Bhatia", Phone = "+919900002001", Email = "rohan.bhatia@easyinsur-demo.com", Addr = "Shop 101, Commercial Arcade, Cyber City, Gurugram, Haryana 122001", Reg = "2024-01-10", Balance = 4850.0 },
    new { Type = "Agent", PersonId = "AGT-2002", First = "Tanya", Last = "Rao", Phone = "+919900002002", Email = "tanya.rao@easyinsur-demo.com", Addr = "Office 402, Apex Business Center, Commercial Street, Bengaluru, Karnataka 560001", Reg = "2024-01-15", Balance = 8420.0 },
    new { Type = "Agent", PersonId = "AGT-2003", First = "Kunal", Last = "Goel", Phone = "+919900002003", Email = "kunal.goel@easyinsur-demo.com", Addr = "Unit 12, Synergy Hub, Drive-in Road, Ahmedabad, Gujarat 380001", Reg = "2024-02-01", Balance = 3200.0 },
    new { Type = "Agent", PersonId = "AGT-2004", First = "Neha", Last = "Menon", Phone = "+919900002004", Email = "neha.menon@easyinsur-demo.com", Addr = "Office 31, Coastal Tower, Port Road, Kochi, Kerala 682001", Reg = "2024-02-20", Balance = 6150.0 },
    new { Type = "Agent", PersonId = "AGT-2005", First = "Varun", Last = "Kapur", Phone = "+919900002005", Email = "varun.kapur@easyinsur-demo.com", Addr = "Suite 5, City Plaza, Sector 17, Chandigarh 160001", Reg = "2024-03-12", Balance = 5300.0 },
    new { Type = "Agent", PersonId = "AGT-2006", First = "Pooja", Last = "Hegde", Phone = "+919900002006", Email = "pooja.hegde@easyinsur-demo.com", Addr = "Chamber 205, Deccan Trade Center, FC Road, Pune, Maharashtra 411001", Reg = "2024-04-01", Balance = 9200.0 },
    new { Type = "Agent", PersonId = "AGT-2007", First = "Siddharth", Last = "Chawla", Phone = "+919900002007", Email = "siddharth.chawla@easyinsur-demo.com", Addr = "Office 18, Central Corporate Park, Park Street, Kolkata, West Bengal 700001", Reg = "2024-04-18", Balance = 2750.0 },
    new { Type = "Agent", PersonId = "AGT-2008", First = "Anjali", Last = "Jain", Phone = "+919900002008", Email = "anjali.jain@easyinsur-demo.com", Addr = "Shop 22, Heritage Trade Center, MI Road, Jaipur, Rajasthan 302001", Reg = "2024-05-05", Balance = 7100.0 },
    new { Type = "Agent", PersonId = "AGT-2009", First = "Mayank", Last = "Mishra", Phone = "+919900002009", Email = "mayank.mishra@easyinsur-demo.com", Addr = "Suite 304, Royal Square, Hazratganj, Lucknow, Uttar Pradesh 226001", Reg = "2024-05-25", Balance = 4500.0 },
    new { Type = "Agent", PersonId = "AGT-2010", First = "Kritika", Last = "Shetty", Phone = "+919900002010", Email = "kritika.shetty@easyinsur-demo.com", Addr = "Level 6, Tech Park Towers, HITEC City, Hyderabad, Telangana 500001", Reg = "2024-06-10", Balance = 11250.0 }
};

var insertedPeople = new List<(long DbId, string PersonId, string Type, string FullName)>();

foreach (var p in syntheticPeople)
{
    using var cmd = connection.CreateCommand();
    cmd.Transaction = transaction;
    cmd.CommandText = """
        INSERT INTO Person (Type, PersonID, Address, Email, FirstName, LastName, Balance, ImagePath, Mobile, RegDate)
        VALUES ($type, $personId, $address, $email, $firstName, $lastName, $balance, $imagePath, $mobile, $regDate);
        SELECT last_insert_rowid();
        """;
    cmd.Parameters.AddWithValue("$type", p.Type);
    cmd.Parameters.AddWithValue("$personId", p.PersonId);
    cmd.Parameters.AddWithValue("$address", p.Addr);
    cmd.Parameters.AddWithValue("$email", p.Email);
    cmd.Parameters.AddWithValue("$firstName", p.First);
    cmd.Parameters.AddWithValue("$lastName", p.Last);
    cmd.Parameters.AddWithValue("$balance", p.Balance);
    cmd.Parameters.AddWithValue("$imagePath", "images/no_image.jpg");
    cmd.Parameters.AddWithValue("$mobile", p.Phone);
    cmd.Parameters.AddWithValue("$regDate", p.Reg);

    var dbId = Convert.ToInt64(cmd.ExecuteScalar());
    insertedPeople.Add((dbId, p.PersonId, p.Type, $"{p.First} {p.Last}"));
}

Console.WriteLine($"Inserted {insertedPeople.Count} synthetic person profiles.");

// ------------------------------------------------------------------------------------------------
// 2. SYNTHETIC VEHICLES & POLICIES (INSURANCE)
// - Fictional registration numbers using standard Indian RTO state patterns with synthetic series (EX)
// - Realistic vehicle valuations (IDVs)
// ------------------------------------------------------------------------------------------------
var syntheticVehicles = new[]
{
    new { VehicleNo = "MH12EX1001", IDV = 890000.0, Reg = "2024-03-10" },  // Compact SUV (Pune)
    new { VehicleNo = "MH14EX1002", IDV = 1350000.0, Reg = "2024-03-25" }, // Mid SUV (PCMC)
    new { VehicleNo = "DL03EX1003", IDV = 1050000.0, Reg = "2024-04-12" }, // Premium Sedan (Delhi)
    new { VehicleNo = "KA01EX1004", IDV = 1950000.0, Reg = "2024-04-28" }, // Luxury SUV (Bengaluru)
    new { VehicleNo = "MH02EX1005", IDV = 720000.0, Reg = "2024-05-15" },  // Compact SUV (Mumbai)
    new { VehicleNo = "GJ01EX1006", IDV = 640000.0, Reg = "2024-05-30" },  // Hatchback (Ahmedabad)
    new { VehicleNo = "HR26EX1007", IDV = 1280000.0, Reg = "2024-06-18" }, // Mid SUV (Gurugram)
    new { VehicleNo = "KA05EX1008", IDV = 2450000.0, Reg = "2024-07-04" }, // MPV (Bengaluru)
    new { VehicleNo = "UP32EX1009", IDV = 3600000.0, Reg = "2024-07-22" }, // Premium 4x4 (Lucknow)
    new { VehicleNo = "TS09EX1010", IDV = 1520000.0, Reg = "2024-08-08" }, // Mid SUV (Hyderabad)
    new { VehicleNo = "MH12EX1011", IDV = 165000.0, Reg = "2024-08-25" },  // Cruiser Motorcycle (Pune)
    new { VehicleNo = "KA04EX1012", IDV = 74000.0, Reg = "2024-09-10" },   // Scooter (Bengaluru)
    new { VehicleNo = "DL08EX1013", IDV = 1120000.0, Reg = "2024-09-27" }, // Compact SUV (Delhi)
    new { VehicleNo = "RJ14EX1014", IDV = 1840000.0, Reg = "2024-10-14" }, // Mid SUV (Jaipur)
    new { VehicleNo = "TN07EX1015", IDV = 810000.0, Reg = "2024-10-31" },  // Compact SUV (Chennai)
    new { VehicleNo = "MH12EX1016", IDV = 82000.0, Reg = "2024-11-15" },   // Scooter 125cc (Pune)
    new { VehicleNo = "KA03EX1017", IDV = 580000.0, Reg = "2024-11-29" },  // Hatchback (Bengaluru)
    new { VehicleNo = "HR29EX1018", IDV = 1250000.0, Reg = "2024-12-14" }, // Executive Sedan (Faridabad)
    new { VehicleNo = "UP16EX1019", IDV = 960000.0, Reg = "2025-01-08" },  // Family MPV (Noida)
    new { VehicleNo = "MH43EX1020", IDV = 520000.0, Reg = "2025-01-22" },  // Commercial Mini Truck (Navi Mumbai)
    new { VehicleNo = "GJ06EX1021", IDV = 610000.0, Reg = "2025-02-05" },  // Micro SUV (Vadodara)
    new { VehicleNo = "DL04EX1022", IDV = 135000.0, Reg = "2025-02-18" },  // Sports Motorcycle (Delhi)
    new { VehicleNo = "TS07EX1023", IDV = 1720000.0, Reg = "2025-02-28" }, // Mid SUV (Hyderabad)
    new { VehicleNo = "KA51EX1024", IDV = 680000.0, Reg = "2025-03-08" }   // Light Commercial Vehicle (Bengaluru)
};

var insertedInsurances = new List<(long DbId, string VehicleNo, double Amount, string RegDate)>();

foreach (var v in syntheticVehicles)
{
    using var cmd = connection.CreateCommand();
    cmd.Transaction = transaction;
    cmd.CommandText = """
        INSERT INTO Insurance (RegDate, VehicleNo, Amount)
        VALUES ($regDate, $vehicleNo, $amount);
        SELECT last_insert_rowid();
        """;
    cmd.Parameters.AddWithValue("$regDate", v.Reg);
    cmd.Parameters.AddWithValue("$vehicleNo", v.VehicleNo);
    cmd.Parameters.AddWithValue("$amount", v.IDV);

    var dbId = Convert.ToInt64(cmd.ExecuteScalar());
    insertedInsurances.Add((dbId, v.VehicleNo, v.IDV, v.Reg));
}

Console.WriteLine($"Inserted {insertedInsurances.Count} synthetic vehicle insurance policies.");

// ------------------------------------------------------------------------------------------------
// 3. SYNTHETIC FINANCIAL TRANSACTIONS & LEDGERS
// - Realistic actuarial insurance formulas (OD, TP, GST, Broker Commission)
// - Linked cleanly between synthetic persons and synthetic vehicles
// ------------------------------------------------------------------------------------------------
var transactionsData = new[]
{
    new { VehicleIdx = 0, PersonIdx = 0, Fixed = 425.0, OD = 14240.0, TP = 3416.0, CommType = "Percentage", ODPct = 12.0, TPPct = 2.5, Date = "2024-03-12", PayFull = true },
    new { VehicleIdx = 1, PersonIdx = 1, Fixed = 425.0, OD = 21600.0, TP = 3416.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.5, Date = "2024-03-26", PayFull = true },
    new { VehicleIdx = 2, PersonIdx = 2, Fixed = 425.0, OD = 16800.0, TP = 3416.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2024-04-14", PayFull = true },
    new { VehicleIdx = 3, PersonIdx = 3, Fixed = 425.0, OD = 31200.0, TP = 7890.0, CommType = "Percentage", ODPct = 12.5, TPPct = 2.5, Date = "2024-04-30", PayFull = true },
    new { VehicleIdx = 4, PersonIdx = 4, Fixed = 425.0, OD = 11520.0, TP = 3416.0, CommType = "Fixed", ODPct = 0.0, TPPct = 0.0, Date = "2024-05-18", PayFull = false },
    new { VehicleIdx = 5, PersonIdx = 5, Fixed = 425.0, OD = 10240.0, TP = 2094.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2024-06-02", PayFull = true },
    new { VehicleIdx = 6, PersonIdx = 6, Fixed = 425.0, OD = 20480.0, TP = 3416.0, CommType = "Percentage", ODPct = 12.0, TPPct = 2.5, Date = "2024-06-20", PayFull = true },
    new { VehicleIdx = 7, PersonIdx = 7, Fixed = 425.0, OD = 39200.0, TP = 7890.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.5, Date = "2024-07-06", PayFull = true },
    new { VehicleIdx = 8, PersonIdx = 8, Fixed = 425.0, OD = 57600.0, TP = 7890.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.5, Date = "2024-07-25", PayFull = true },
    new { VehicleIdx = 9, PersonIdx = 9, Fixed = 425.0, OD = 24320.0, TP = 7890.0, CommType = "Percentage", ODPct = 12.0, TPPct = 2.5, Date = "2024-08-10", PayFull = true },
    new { VehicleIdx = 10, PersonIdx = 10, Fixed = 375.0, OD = 2310.0, TP = 1366.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.0, Date = "2024-08-28", PayFull = true },
    new { VehicleIdx = 11, PersonIdx = 11, Fixed = 375.0, OD = 1036.0, TP = 714.0, CommType = "Fixed", ODPct = 0.0, TPPct = 0.0, Date = "2024-09-12", PayFull = false },
    new { VehicleIdx = 12, PersonIdx = 12, Fixed = 425.0, OD = 17920.0, TP = 3416.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2024-09-30", PayFull = true },
    new { VehicleIdx = 13, PersonIdx = 13, Fixed = 425.0, OD = 29440.0, TP = 7890.0, CommType = "Percentage", ODPct = 12.5, TPPct = 2.5, Date = "2024-10-16", PayFull = true },
    new { VehicleIdx = 14, PersonIdx = 14, Fixed = 425.0, OD = 12960.0, TP = 2094.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2024-11-02", PayFull = true },
    new { VehicleIdx = 15, PersonIdx = 15, Fixed = 375.0, OD = 1148.0, TP = 714.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.0, Date = "2024-11-18", PayFull = true },
    new { VehicleIdx = 16, PersonIdx = 16, Fixed = 425.0, OD = 9280.0, TP = 2094.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2024-12-02", PayFull = true },
    new { VehicleIdx = 17, PersonIdx = 17, Fixed = 425.0, OD = 20000.0, TP = 3416.0, CommType = "Percentage", ODPct = 12.0, TPPct = 2.5, Date = "2024-12-16", PayFull = true },
    new { VehicleIdx = 18, PersonIdx = 18, Fixed = 425.0, OD = 15360.0, TP = 3416.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2025-01-10", PayFull = true },
    new { VehicleIdx = 19, PersonIdx = 19, Fixed = 425.0, OD = 10400.0, TP = 15746.0, CommType = "Percentage", ODPct = 8.0, TPPct = 2.0, Date = "2025-01-25", PayFull = true },
    // Agent Commission Entries
    new { VehicleIdx = 20, PersonIdx = 20, Fixed = 425.0, OD = 9760.0, TP = 2094.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.5, Date = "2025-02-08", PayFull = true },
    new { VehicleIdx = 21, PersonIdx = 21, Fixed = 375.0, OD = 1890.0, TP = 1366.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.0, Date = "2025-02-20", PayFull = true },
    new { VehicleIdx = 22, PersonIdx = 22, Fixed = 425.0, OD = 27520.0, TP = 7890.0, CommType = "Percentage", ODPct = 12.5, TPPct = 2.5, Date = "2025-03-02", PayFull = true },
    new { VehicleIdx = 23, PersonIdx = 23, Fixed = 425.0, OD = 13600.0, TP = 15746.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.0, Date = "2025-03-10", PayFull = true },
    new { VehicleIdx = 0, PersonIdx = 24, Fixed = 425.0, OD = 14240.0, TP = 3416.0, CommType = "Percentage", ODPct = 12.0, TPPct = 2.5, Date = "2025-03-15", PayFull = true },
    new { VehicleIdx = 1, PersonIdx = 25, Fixed = 425.0, OD = 21600.0, TP = 3416.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.5, Date = "2025-03-18", PayFull = true },
    new { VehicleIdx = 2, PersonIdx = 26, Fixed = 425.0, OD = 16800.0, TP = 3416.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2025-03-20", PayFull = true },
    new { VehicleIdx = 3, PersonIdx = 27, Fixed = 425.0, OD = 31200.0, TP = 7890.0, CommType = "Percentage", ODPct = 12.5, TPPct = 2.5, Date = "2025-03-22", PayFull = true },
    new { VehicleIdx = 4, PersonIdx = 28, Fixed = 425.0, OD = 11520.0, TP = 3416.0, CommType = "Percentage", ODPct = 10.0, TPPct = 2.5, Date = "2025-03-25", PayFull = true },
    new { VehicleIdx = 8, PersonIdx = 29, Fixed = 425.0, OD = 57600.0, TP = 7890.0, CommType = "Percentage", ODPct = 15.0, TPPct = 2.5, Date = "2025-03-28", PayFull = true }
};

int txCount = 0;
foreach (var t in transactionsData)
{
    var vehicle = insertedInsurances[t.VehicleIdx];
    var person = insertedPeople[t.PersonIdx];

    var netPremium = t.Fixed + t.OD + t.TP;
    var tax = Math.Round(netPremium * 0.18, 2);
    var totalAmount = Math.Round(netPremium + tax, 2);

    double commission;
    if (t.CommType == "Percentage")
    {
        commission = Math.Round((t.OD * (t.ODPct / 100.0)) + (t.TP * (t.TPPct / 100.0)), 2);
    }
    else
    {
        commission = 1500.0;
    }

    var afterCommission = Math.Round(totalAmount - commission, 2);
    double payment;
    double balance;

    if (person.Type == "Customer")
    {
        if (t.PayFull)
        {
            payment = totalAmount;
            balance = 0.0;
        }
        else
        {
            payment = Math.Round(totalAmount - 1250.0, 2);
            balance = 1250.0;
        }
    }
    else
    {
        payment = afterCommission;
        balance = commission;
    }

    using var cmd = connection.CreateCommand();
    cmd.Transaction = transaction;
    cmd.CommandText = """
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
    cmd.Parameters.AddWithValue("$insuranceId", vehicle.DbId);
    cmd.Parameters.AddWithValue("$personId", person.DbId);
    cmd.Parameters.AddWithValue("$fixedAmount", t.Fixed);
    cmd.Parameters.AddWithValue("$od", t.OD);
    cmd.Parameters.AddWithValue("$tp", t.TP);
    cmd.Parameters.AddWithValue("$tax", tax);
    cmd.Parameters.AddWithValue("$total", totalAmount);
    cmd.Parameters.AddWithValue("$commissionType", t.CommType);
    cmd.Parameters.AddWithValue("$commissionAmount", commission);
    cmd.Parameters.AddWithValue("$odPercent", t.ODPct);
    cmd.Parameters.AddWithValue("$tpPercent", t.TPPct);
    cmd.Parameters.AddWithValue("$afterCommissionAmount", afterCommission);
    cmd.Parameters.AddWithValue("$payment", payment);
    cmd.Parameters.AddWithValue("$paymentDate", t.Date);
    cmd.Parameters.AddWithValue("$balance", balance);
    cmd.Parameters.AddWithValue("$previousBalance", 0.0);
    cmd.Parameters.AddWithValue("$finalBalance", balance);

    cmd.ExecuteNonQuery();
    txCount++;
}

transaction.Commit();

Console.WriteLine($"SUCCESS: Seeded database with {insertedPeople.Count} synthetic profiles, {insertedInsurances.Count} synthetic policies, and {txCount} financial transactions.");

// Print sample for verification
Console.WriteLine("\n--- VERIFICATION: SAMPLE SYNTHETIC DATA ---");
using (var vCmd = connection.CreateCommand())
{
    vCmd.CommandText = "SELECT Type, PersonID, FirstName || ' ' || LastName AS Name, Mobile, Email, Address FROM Person LIMIT 3;";
    using var reader = vCmd.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"[{reader["Type"]}] {reader["PersonID"]}: {reader["Name"]} | Phone: {reader["Mobile"]} | Email: {reader["Email"]}");
        Console.WriteLine($"       Address: {reader["Address"]}");
    }
}
using (var vCmd = connection.CreateCommand())
{
    vCmd.CommandText = "SELECT VehicleNo, Amount, RegDate FROM Insurance LIMIT 3;";
    using var reader = vCmd.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"Vehicle: {reader["VehicleNo"]} | IDV: INR {reader["Amount"]:N2} | Date: {reader["RegDate"]}");
    }
}
