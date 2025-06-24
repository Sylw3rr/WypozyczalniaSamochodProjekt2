namespace CarRentalSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year - (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);

        // DODANO BRAKUJ¥C¥ METODÊ
        public bool CanRent() => Age >= 18 && Age <= 75;

        public Customer() { }

        public Customer(int id, string firstName, string lastName, string email, string phone, DateTime dob)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            DateOfBirth = dob;
        }
    }
}