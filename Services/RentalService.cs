using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;

namespace CarRentalSystem.Services
{
    public class RentalService : IRentalService
    {
        private readonly IVehicleService _vehicleService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly List<Rental> _rentals = new List<Rental>();
        private int _nextId = 1;

        public RentalService(IVehicleService vehicleService, ICustomerService customerService, ILogger logger)
        {
            _vehicleService = vehicleService;
            _customerService = customerService;
            _logger = logger;
        }

        public Rental CreateRental(int vehicleId, int customerId, DateTime startDate, DateTime endDate)
        {
            var vehicle = _vehicleService.GetVehicleById(vehicleId);
            var customer = _customerService.GetCustomerById(customerId);

            if (vehicle == null || customer == null) throw new InvalidOperationException("Pojazd lub klient nie istnieje.");
            if (!vehicle.IsAvailable) throw new InvalidOperationException("Pojazd nie jest dostêpny.");
            if (!customer.CanRent()) throw new InvalidOperationException("Klient nie spe³nia warunków wynajmu.");
            if (startDate >= endDate) throw new InvalidOperationException("Data koñcowa musi byæ póŸniejsza ni¿ pocz¹tkowa.");

            var rental = new Rental
            {
                Id = _nextId++,
                VehicleId = vehicleId,
                CustomerId = customerId,
                StartDate = startDate,
                EndDate = endDate,
                Status = RentalStatus.Active,
                TotalCost = (decimal)(endDate - startDate).TotalDays * vehicle.DailyRate
            };

            _rentals.Add(rental);
            _vehicleService.SetVehicleAvailability(vehicleId, false);
            _logger.LogInfo($"Utworzono wynajem {rental.Id} dla pojazdu {vehicle.Id} przez klienta {customer.Id}");
            return rental;
        }

        public void EndRental(int rentalId, DateTime returnDate)
        {
            var rental = _rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null) throw new InvalidOperationException("Wynajem nie istnieje.");

            rental.ActualReturnDate = returnDate;
            rental.Status = RentalStatus.Completed;
            _vehicleService.SetVehicleAvailability(rental.VehicleId, true);
            _logger.LogInfo($"Zakoñczono wynajem {rental.Id}");
        }

        public IEnumerable<Rental> GetAllRentals() => _rentals;

        public Rental GetActiveRental(int vehicleId)
        {
            return _rentals.FirstOrDefault(r => r.VehicleId == vehicleId && r.Status == RentalStatus.Active);
        }
    }
}