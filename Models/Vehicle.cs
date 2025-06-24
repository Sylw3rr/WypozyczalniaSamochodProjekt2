namespace CarRentalSystem.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; } // DODANO BRAKUJ¥C¥ W£AŒCIWOŒÆ
        public decimal DailyRate { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; }

        public Vehicle()
        {
            IsAvailable = true;
        }

        public Vehicle(int id, string make, string model, int year, string licensePlate, decimal dailyRate, string category)
        {
            Id = id;
            Make = make;
            Model = model;
            Year = year;
            LicensePlate = licensePlate; // DODANO
            DailyRate = dailyRate;
            Category = category;
            IsAvailable = true;
        }

        public override string ToString() => $"{Make} {Model} ({Year}) - {LicensePlate}";
    }
}