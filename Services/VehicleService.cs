using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;

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
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            AddVehicle(new Vehicle(0, "Toyota", "Corolla", 2022, "WPR 12345", 150, "Sedan"));
            AddVehicle(new Vehicle(0, "Ford", "Mustang", 2023, "GD 54321", 400, "Sport"));
            AddVehicle(new Vehicle(0, "BMW", "X5", 2021, "KR 11223", 350, "SUV"));
        }

        public void AddVehicle(Vehicle vehicle)
        {
            vehicle.Id = _nextId++;
            _vehicles.Add(vehicle);
            _logger.LogInfo($"Dodano pojazd: {vehicle}");
        }

        public void DeleteVehicle(int id)
        {
            var vehicle = GetVehicleById(id);
            if (vehicle != null)
            {
                _vehicles.Remove(vehicle);
                _logger.LogInfo($"Usuniêto pojazd: {vehicle}");
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
                _logger.LogInfo($"Zmieniono dostêpnoœæ pojazdu {vehicle.Id} na {isAvailable}");
            }
        }
        public void UpdateVehicle(Vehicle vehicle)
        {
            var existing = GetVehicleById(vehicle.Id);
            if (existing != null)
            {
                existing.Make = vehicle.Make;
                existing.Model = vehicle.Model;
                existing.Year = vehicle.Year;
                existing.LicensePlate = vehicle.LicensePlate;
                existing.DailyRate = vehicle.DailyRate;
                existing.Category = vehicle.Category;
                _logger.LogInfo($"Zaktualizowano pojazd: {existing}");
            }
        }
    }
}