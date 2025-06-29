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

            // ✅ WAŻNE: Inicjalizuj bazę danych PRZED utworzeniem serwisów
            Database.InitializeDatabase();

            // Dependency Injection
            ILogger logger = Logger.Instance;
            IVehicleService vehicleService = new VehicleService(logger);
            ICustomerService customerService = new CustomerService(logger);
            IRentalService rentalService = new RentalService(vehicleService, customerService, logger);

            // Wczytaj dane z bazy danych
            vehicleService.LoadVehiclesFromDb();
            customerService.LoadCustomersFromDb();
            rentalService.LoadRentalsFromDb();

            Application.Run(new MainForm(vehicleService, customerService, rentalService, logger));
        }
    }
}