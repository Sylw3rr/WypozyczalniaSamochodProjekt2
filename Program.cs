using System;
using System.Windows.Forms;
using CarRentalSystem.Forms;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Services;
using CarRentalSystem.Utils;
using System.Data.SQLite;

namespace CarRentalSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Inicjalizacja bazy danych z migracją
            Database.InitializeDatabase();

            // Dependency Injection
            ILogger logger = Logger.Instance;
            IVehicleService vehicleService = new VehicleService(logger);
            ICustomerService customerService = new CustomerService(logger);
            IRentalService rentalService = new RentalService(vehicleService, customerService, logger);

            // Wczytanie danych z obsługą błędów
            try
            {
                vehicleService.LoadVehiclesFromDb();
                customerService.LoadCustomersFromDb();
                rentalService.LoadRentalsFromDb();
            }
            catch (Exception ex)
            {
                logger.LogError("Błąd wczytywania danych", ex);
            }

            Application.Run(new MainForm(vehicleService, customerService, rentalService, logger));
        }
    }
}