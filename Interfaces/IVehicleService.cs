using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;       // dla SQLiteConnection, SQLiteCommand
using CarRentalSystem.Models;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Utils;
using System.Windows.Forms;

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
        void LoadVehiclesFromDb();
    }
}