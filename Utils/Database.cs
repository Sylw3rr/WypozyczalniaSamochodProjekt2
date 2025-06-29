using System.Data.SQLite;
using System.IO;

namespace CarRentalSystem.Utils
{
    public static class Database
    {
        // Œcie¿ka do pliku bazy danych w katalogu aplikacji
        private static readonly string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "carrental.db");
        private static readonly string connectionString = $"Data Source={dbPath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            // SprawdŸ, czy baza danych istnieje, jeœli nie - utwórz j¹
            InitializeDatabase();

            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Tworzy bazê danych i tabele przy pierwszym uruchomieniu
        /// </summary>
        public static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables();
            }
            else
            {
                EnsureAdditionalChargesColumn();
            }
        }
        private static void EnsureAdditionalChargesColumn()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                var cmd = new SQLiteCommand("PRAGMA table_info(Rentals);", conn);
                bool hasColumn = false;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["name"].ToString() == "AdditionalCharges")
                        {
                            hasColumn = true;
                            break;
                        }
                    }
                }

                if (!hasColumn)
                {
                    var alterCmd = new SQLiteCommand(
                        "ALTER TABLE Rentals ADD COLUMN AdditionalCharges REAL NOT NULL DEFAULT 0;", conn);
                    alterCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Tworzy wszystkie wymagane tabele w bazie danych
        /// </summary>
        private static void CreateTables()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Tabela Vehicles
                string createVehiclesTable = @"
                CREATE TABLE IF NOT EXISTS Vehicles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Make TEXT NOT NULL,
                    Model TEXT NOT NULL,
                    Year INTEGER NOT NULL,
                    LicensePlate TEXT NOT NULL,
                    DailyRate REAL NOT NULL,
                    Category TEXT NOT NULL,
                    IsAvailable BOOLEAN NOT NULL DEFAULT 1
                );";

                // Tabela Customers
                string createCustomersTable = @"
                CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    Email TEXT,
                    PhoneNumber TEXT,
                    DateOfBirth TEXT NOT NULL
                );";

                // Tabela Rentals
                string createRentalsTable = @"
        CREATE TABLE IF NOT EXISTS Rentals (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            VehicleId INTEGER NOT NULL,
            CustomerId INTEGER NOT NULL,
            StartDate TEXT NOT NULL,
            EndDate TEXT NOT NULL,
            ActualReturnDate TEXT,
            Status TEXT NOT NULL,
            TotalCost REAL NOT NULL,
            AdditionalCharges REAL NOT NULL DEFAULT 0,
            FOREIGN KEY (VehicleId) REFERENCES Vehicles (Id),
            FOREIGN KEY (CustomerId) REFERENCES Customers (Id)
        );";

                // Wykonaj wszystkie polecenia CREATE TABLE
                ExecuteCommand(conn, createVehiclesTable);
                ExecuteCommand(conn, createCustomersTable);
                ExecuteCommand(conn, createRentalsTable);
            }
        }

        /// <summary>
        /// Pomocnicza metoda do wykonywania poleceñ SQL
        /// </summary>
        private static void ExecuteCommand(SQLiteConnection connection, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}