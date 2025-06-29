using System;

namespace CarRentalSystem.Models
{
    public enum RentalStatus
    {
        Pending,     // Oczekuj¹ce
        Active,      // Aktywne
        Completed,   // Zakoñczone
        Cancelled    // Anulowane
    }

    public class Rental
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public RentalStatus Status { get; set; } = RentalStatus.Pending;
        public decimal TotalCost { get; set; }
        public decimal AdditionalCharges { get; set; } = 0;

        // Obliczane w³aœciwoœci
        public int PlannedDays => (EndDate - StartDate).Days + 1;
        public int? ActualDays => ActualReturnDate?.Subtract(StartDate).Days + 1;
        public bool IsOverdue => Status == RentalStatus.Active && DateTime.Now > EndDate;
        public decimal FinalCost => TotalCost + AdditionalCharges;

        public override string ToString()
        {
            return $"Wypo¿yczenie #{Id} - Status: {Status}";
        }
    }
}