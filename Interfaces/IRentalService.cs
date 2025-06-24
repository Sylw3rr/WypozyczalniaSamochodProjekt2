using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface IRentalService
    {
        Rental CreateRental(int vehicleId, int customerId, DateTime startDate, DateTime endDate);
        void EndRental(int rentalId, DateTime returnDate);
        IEnumerable<Rental> GetAllRentals();
        Rental GetActiveRental(int vehicleId); // DODANO BRAKUJ¥C¥ METODÊ
    }
}