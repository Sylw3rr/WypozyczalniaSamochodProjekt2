using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;       // dla SQLiteConnection, SQLiteCommand
using CarRentalSystem.Models;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Utils;
using System.Windows.Forms;
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
            if (!vehicle.IsAvailable) throw new InvalidOperationException("Pojazd nie jest dost�pny.");
            if (!customer.CanRent()) throw new InvalidOperationException("Klient nie spe�nia warunk�w wynajmu.");
            if (startDate >= endDate) throw new InvalidOperationException("Data ko�cowa musi by� p�niejsza ni� pocz�tkowa.");

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
            _logger.LogInfo($"Zako�czono wynajem {rental.Id}");
        }

        public IEnumerable<Rental> GetAllRentals() => _rentals;

        public Rental GetActiveRental(int vehicleId)
        {
            return _rentals.FirstOrDefault(r => r.VehicleId == vehicleId && r.Status == RentalStatus.Active);
        }
        public void SaveRentalToDb(Rental rental)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("INSERT INTO Rentals (VehicleId, CustomerId, StartDate, EndDate, Status) VALUES (@VehicleId, @CustomerId, @StartDate, @EndDate, @Status)", conn);
                cmd.Parameters.AddWithValue("@VehicleId", rental.VehicleId);
                cmd.Parameters.AddWithValue("@CustomerId", rental.CustomerId);
                cmd.Parameters.AddWithValue("@StartDate", rental.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", rental.EndDate);
                cmd.Parameters.AddWithValue("@Status", rental.Status.ToString());
                cmd.ExecuteNonQuery();
            }
        }
        public void LoadRentalsFromDb()
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Rentals", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var rental = new Rental
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            VehicleId = Convert.ToInt32(reader["VehicleId"]),
                            CustomerId = Convert.ToInt32(reader["CustomerId"]),
                            StartDate = DateTime.Parse(reader["StartDate"].ToString()),
                            EndDate = DateTime.Parse(reader["EndDate"].ToString()),
                            Status = (RentalStatus)Enum.Parse(typeof(RentalStatus), reader["Status"].ToString())
                        };
                        _rentals.Add(rental);
                    }
                }
            }
        }


    }
}