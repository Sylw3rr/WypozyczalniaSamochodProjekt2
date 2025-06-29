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
    public interface IRentalService
    {
        Rental CreateRental(int vehicleId, int customerId, DateTime startDate, DateTime endDate);
        void EndRental(int rentalId, DateTime returnDate);
        IEnumerable<Rental> GetAllRentals();
        void LoadRentalsFromDb();
        Rental GetActiveRental(int vehicleId); // DODANO BRAKUJ¥C¥ METODÊ
    }
}