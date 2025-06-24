using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface IVehicleService
    {
        IEnumerable<Vehicle> GetAllVehicles();
        IEnumerable<Vehicle> GetAvailableVehicles();
        Vehicle GetVehicleById(int id);
        void AddVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(int id);
        void SetVehicleAvailability(int id, bool isAvailable);
    }
}