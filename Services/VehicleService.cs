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
    public class VehicleService : IVehicleService
    {
        private readonly ILogger _logger;
        private readonly List<Vehicle> _vehicles;
        private int _nextId = 1;

        public VehicleService(ILogger logger)
        {
            _logger = logger;
            _vehicles = new List<Vehicle>();
        }


        public void AddVehicle(Vehicle vehicle)
        {
            vehicle.Id = _nextId++;
            _vehicles.Add(vehicle);
            SaveVehicleToDb(vehicle);
            _logger.LogInfo($"Dodano pojazd: {vehicle}");
        }

        public void DeleteVehicleFromDb(int id)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("DELETE FROM Vehicles WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteVehicle(int id)
        {
            var vehicle = GetVehicleById(id);
            if (vehicle != null)
            {
                _vehicles.Remove(vehicle);
                DeleteVehicleFromDb(id); // ← DODAJ TĘ LINIĘ
                _logger.LogInfo($"Usunięto pojazd: {vehicle}");
            }
        }

        public IEnumerable<Vehicle> GetAllVehicles() => _vehicles;
        public IEnumerable<Vehicle> GetAvailableVehicles() => _vehicles.Where(v => v.IsAvailable);
        public Vehicle GetVehicleById(int id) => _vehicles.FirstOrDefault(v => v.Id == id);
        public void SetVehicleAvailability(int id, bool isAvailable)
        {
            var vehicle = GetVehicleById(id);
            if (vehicle != null)
            {
                vehicle.IsAvailable = isAvailable;
                SaveVehicleToDb(vehicle);
                _logger.LogInfo($"Zmieniono dostępność pojazdu {vehicle.Id} na {isAvailable}");
            }
        }
        public void UpdateVehicle(Vehicle vehicle)
        {
            var existingVehicle = _vehicles.FirstOrDefault(v => v.Id == vehicle.Id);
            if (existingVehicle != null)
            {
                
                existingVehicle.Make = vehicle.Make;
                existingVehicle.Model = vehicle.Model;
                existingVehicle.Year = vehicle.Year;
                existingVehicle.LicensePlate = vehicle.LicensePlate;
                existingVehicle.DailyRate = vehicle.DailyRate;
                existingVehicle.Category = vehicle.Category;
                SaveVehicleToDb(existingVehicle);
                _logger.LogInfo($"Zaktualizowano pojazd: {existingVehicle}");

            }
        }
        public void SaveVehicleToDb(Vehicle vehicle)
        {
            using (var conn = Database.GetConnection())
            {
                // "INSERT OR REPLACE" działa tak, że jeśli pojazd o danym ID już istnieje, zostanie zaktualizowany.
                // Jeśli nie istnieje, zostanie dodany jako nowy.
                var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Vehicles (Id, Make, Model, Year, LicensePlate, DailyRate, Category, IsAvailable) VALUES (@Id, @Make, @Model, @Year, @LicensePlate, @DailyRate, @Category, @IsAvailable)", conn);
                cmd.Parameters.AddWithValue("@Id", vehicle.Id);
                cmd.Parameters.AddWithValue("@Make", vehicle.Make);
                cmd.Parameters.AddWithValue("@Model", vehicle.Model);
                cmd.Parameters.AddWithValue("@Year", vehicle.Year);
                cmd.Parameters.AddWithValue("@LicensePlate", vehicle.LicensePlate);
                cmd.Parameters.AddWithValue("@DailyRate", vehicle.DailyRate);
                cmd.Parameters.AddWithValue("@Category", vehicle.Category);
                cmd.Parameters.AddWithValue("@IsAvailable", vehicle.IsAvailable); // SQLite obsłuży bool jako 0 lub 1
                cmd.ExecuteNonQuery();
            }
        }

        // Metoda do wczytania wszystkich pojazdów z bazy przy starcie aplikacji
        public void LoadVehiclesFromDb()
        {
            _vehicles.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Vehicles", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var vehicle = new Vehicle(
                            id: Convert.ToInt32(reader["Id"]),
                            make: reader["Make"].ToString(),
                            model: reader["Model"].ToString(),
                            year: Convert.ToInt32(reader["Year"]),
                            licensePlate: reader["LicensePlate"].ToString(),
                            dailyRate: Convert.ToDecimal(reader["DailyRate"]),
                            category: reader["Category"].ToString()
                        )
                        {
                            IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                        };
                        _vehicles.Add(vehicle);
                    }
                }
            }
            if (_vehicles.Any())
                _nextId = _vehicles.Max(v => v.Id) + 1;
        }
    }
}