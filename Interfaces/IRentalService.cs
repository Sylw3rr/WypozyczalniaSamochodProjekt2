using System;
using System.Collections.Generic;
using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface IRentalService
    {
        // Podstawowe operacje CRUD
        Rental CreateRental(int vehicleId, int customerId, DateTime startDate, DateTime endDate);
        void UpdateRental(Rental rental);
        bool DeleteRental(int rentalId);
        Rental GetRentalById(int rentalId);
        IEnumerable<Rental> GetAllRentals();

        // Operacje biznesowe
        void EndRental(int rentalId, DateTime returnDate, decimal additionalCharges = 0);
        void CancelRental(int rentalId);
        Rental GetActiveRental(int vehicleId);

        // Zapytania filtruj¹ce
        IEnumerable<Rental> GetActiveRentals();
        IEnumerable<Rental> GetOverdueRentals();
        IEnumerable<Rental> GetRentalsByCustomer(int customerId);
        IEnumerable<Rental> GetRentalsByVehicle(int vehicleId);
        IEnumerable<Rental> GetRentalsByStatus(RentalStatus status);

        // Operacje bazodanowe
        void SaveRentalToDb(Rental rental);
        void DeleteRentalFromDb(int rentalId);
        void LoadRentalsFromDb();
    }
}