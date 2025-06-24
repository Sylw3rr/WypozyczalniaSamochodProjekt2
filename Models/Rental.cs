namespace CarRentalSystem.Models
{
    public enum RentalStatus { Pending, Active, Completed, Cancelled }

    public class Rental
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal? AdditionalCharges { get; set; }
        public RentalStatus Status { get; set; }

        public Rental()
        {
            Status = RentalStatus.Pending;
        }
    }
}