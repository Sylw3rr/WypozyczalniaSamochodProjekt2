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

            // Podstawowa walidacja
            if (vehicle == null || customer == null)
                throw new InvalidOperationException("Pojazd lub klient nie istnieje.");
            if (!vehicle.IsAvailable)
                throw new InvalidOperationException("Pojazd nie jest dostêpny.");
            if (!customer.CanRent())
                throw new InvalidOperationException("Klient nie spe³nia warunków wynajmu.");
            if (startDate >= endDate)
                throw new InvalidOperationException("Data koñcowa musi byæ póŸniejsza ni¿ pocz¹tkowa.");
            if (startDate < DateTime.Today)
                throw new InvalidOperationException("Data rozpoczêcia nie mo¿e byæ w przesz³oœci.");

            // SprawdŸ konflikty czasowe
            var conflictingRental = _rentals.FirstOrDefault(r =>
                r.VehicleId == vehicleId &&
                r.Status == RentalStatus.Active &&
                ((startDate >= r.StartDate && startDate <= r.EndDate) ||
                 (endDate >= r.StartDate && endDate <= r.EndDate) ||
                 (startDate <= r.StartDate && endDate >= r.EndDate)));

            if (conflictingRental != null)
                throw new InvalidOperationException($"Pojazd jest ju¿ wypo¿yczony w tym terminie (wypo¿yczenie #{conflictingRental.Id}).");

            var rental = new Rental
            {
                Id = _nextId++,
                VehicleId = vehicleId,
                CustomerId = customerId,
                StartDate = startDate,
                EndDate = endDate,
                Status = RentalStatus.Active,
                TotalCost = (decimal)(endDate - startDate).TotalDays * vehicle.DailyRate,
                AdditionalCharges = 0
            };

            _rentals.Add(rental);
            SaveRentalToDb(rental);
            _vehicleService.SetVehicleAvailability(vehicleId, false);
            _logger.LogInfo($"Utworzono wypo¿yczenie {rental.Id} dla pojazdu {vehicle.Id} przez klienta {customer.Id}");

            return rental;
        }

        public void EndRental(int rentalId, DateTime returnDate, decimal additionalCharges = 0)
        {
            var rental = _rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null)
                throw new InvalidOperationException("Wypo¿yczenie nie istnieje.");

            if (rental.Status != RentalStatus.Active)
                throw new InvalidOperationException("Mo¿na zakoñczyæ tylko aktywne wypo¿yczenia.");

            rental.ActualReturnDate = returnDate;
            rental.Status = RentalStatus.Completed;
            rental.AdditionalCharges = additionalCharges;

            // Oblicz kary za opóŸnienie
            if (returnDate.Date > rental.EndDate.Date)
            {
                var vehicle = _vehicleService.GetVehicleById(rental.VehicleId);
                if (vehicle != null)
                {
                    var lateDays = (returnDate.Date - rental.EndDate.Date).Days;
                    var lateFee = lateDays * vehicle.DailyRate * 1.5m; // 150% normalnej stawki jako kara
                    rental.AdditionalCharges += lateFee;
                    _logger.LogWarning($"Wypo¿yczenie {rental.Id} zwrócone z opóŸnieniem {lateDays} dni. Kara: {lateFee:C}");
                }
            }

            _vehicleService.SetVehicleAvailability(rental.VehicleId, true);
            SaveRentalToDb(rental);
            _logger.LogInfo($"Zakoñczono wypo¿yczenie {rental.Id}. Ca³kowity koszt: {(rental.TotalCost + rental.AdditionalCharges):C}");
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
                var cmd = new SQLiteCommand(@"
            INSERT OR REPLACE INTO Rentals 
            (Id, VehicleId, CustomerId, StartDate, EndDate, ActualReturnDate, Status, TotalCost, AdditionalCharges) 
            VALUES 
            (@Id, @VehicleId, @CustomerId, @StartDate, @EndDate, @ActualReturnDate, @Status, @TotalCost, @AdditionalCharges)", conn);

                cmd.Parameters.AddWithValue("@Id", rental.Id);
                cmd.Parameters.AddWithValue("@VehicleId", rental.VehicleId);
                cmd.Parameters.AddWithValue("@CustomerId", rental.CustomerId);
                cmd.Parameters.AddWithValue("@StartDate", rental.StartDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@EndDate", rental.EndDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@ActualReturnDate",
                    rental.ActualReturnDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", rental.Status.ToString());
                cmd.Parameters.AddWithValue("@TotalCost", rental.TotalCost);
                cmd.Parameters.AddWithValue("@AdditionalCharges", rental.AdditionalCharges);

                cmd.ExecuteNonQuery();
            }
        }

        public void LoadRentalsFromDb()
        {
            _rentals.Clear();
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
                            ActualReturnDate = reader["ActualReturnDate"] == DBNull.Value ?
                                null : DateTime.Parse(reader["ActualReturnDate"].ToString()),
                            Status = (RentalStatus)Enum.Parse(typeof(RentalStatus), reader["Status"].ToString()),
                            TotalCost = Convert.ToDecimal(reader["TotalCost"]),
                            AdditionalCharges = HasColumn(reader, "AdditionalCharges") && reader["AdditionalCharges"] != DBNull.Value ?
                                Convert.ToDecimal(reader["AdditionalCharges"]) : 0
                        };
                        _rentals.Add(rental);
                    }
                }
            }

            if (_rentals.Any())
            {
                _nextId = _rentals.Max(r => r.Id) + 1;
            }
        }

        // Pomocnicza metoda do sprawdzania istnienia kolumny
        private bool HasColumn(IDataReader reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
        public void UpdateRental(Rental rental)
        {
            var existingRental = _rentals.FirstOrDefault(r => r.Id == rental.Id);
            if (existingRental == null)
                throw new InvalidOperationException("Wypo¿yczenie nie istnieje.");

            // Aktualizuj w³aœciwoœci
            existingRental.VehicleId = rental.VehicleId;
            existingRental.CustomerId = rental.CustomerId;
            existingRental.StartDate = rental.StartDate;
            existingRental.EndDate = rental.EndDate;
            existingRental.ActualReturnDate = rental.ActualReturnDate;
            existingRental.Status = rental.Status;
            existingRental.TotalCost = rental.TotalCost;
            existingRental.AdditionalCharges = rental.AdditionalCharges;

            SaveRentalToDb(existingRental);
            _logger.LogInfo($"Zaktualizowano wypo¿yczenie {existingRental.Id}");
        }

        public bool DeleteRental(int rentalId)
        {
            var rental = _rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null) return false;

            _rentals.Remove(rental);
            DeleteRentalFromDb(rentalId);

            // Przywróæ dostêpnoœæ pojazdu jeœli wypo¿yczenie by³o aktywne
            if (rental.Status == RentalStatus.Active)
            {
                _vehicleService.SetVehicleAvailability(rental.VehicleId, true);
            }

            _logger.LogInfo($"Usuniêto wypo¿yczenie {rental.Id}");
            return true;
        }

        public Rental GetRentalById(int rentalId)
        {
            return _rentals.FirstOrDefault(r => r.Id == rentalId);
        }
        public void CancelRental(int rentalId)
        {
            var rental = _rentals.FirstOrDefault(r => r.Id == rentalId);
            if (rental == null)
                throw new InvalidOperationException("Wypo¿yczenie nie istnieje.");

            rental.Status = RentalStatus.Cancelled;
            _vehicleService.SetVehicleAvailability(rental.VehicleId, true);
            SaveRentalToDb(rental);
            _logger.LogInfo($"Anulowano wypo¿yczenie {rental.Id}");
        }
        public IEnumerable<Rental> GetActiveRentals()
        {
            return _rentals.Where(r => r.Status == RentalStatus.Active);
        }

        public IEnumerable<Rental> GetOverdueRentals()
        {
            return _rentals.Where(r => r.Status == RentalStatus.Active && r.EndDate < DateTime.Now);
        }

        public IEnumerable<Rental> GetRentalsByCustomer(int customerId)
        {
            return _rentals.Where(r => r.CustomerId == customerId);
        }

        public IEnumerable<Rental> GetRentalsByVehicle(int vehicleId)
        {
            return _rentals.Where(r => r.VehicleId == vehicleId);
        }

        public IEnumerable<Rental> GetRentalsByStatus(RentalStatus status)
        {
            return _rentals.Where(r => r.Status == status);
        }
        public void DeleteRentalFromDb(int rentalId)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("DELETE FROM Rentals WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", rentalId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}