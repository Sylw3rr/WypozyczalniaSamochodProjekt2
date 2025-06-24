using CarRentalSystem.Forms;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Services;
using CarRentalSystem.Utils;

namespace CarRentalSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // === Dependency Injection ===
            ILogger logger = Logger.Instance;
            IVehicleService vehicleService = new VehicleService(logger);
            ICustomerService customerService = new CustomerService(logger);
            IRentalService rentalService = new RentalService(vehicleService, customerService, logger);

            Application.Run(new MainForm(vehicleService, customerService, rentalService, logger));
        }
    }
}