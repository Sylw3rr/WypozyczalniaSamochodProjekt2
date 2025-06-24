using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;

namespace CarRentalSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger _logger;
        private readonly List<Customer> _customers;
        private int _nextId = 1;

        public CustomerService(ILogger logger)
        {
            _logger = logger;
            _customers = new List<Customer>();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            AddCustomer(new Customer(0, "Jan", "Kowalski", "jan.k@email.com", "123456789", new DateTime(1990, 5, 15)));
            AddCustomer(new Customer(0, "Anna", "Nowak", "anna.n@email.com", "987654321", new DateTime(1985, 10, 25)));
        }

        public void AddCustomer(Customer customer)
        {
            customer.Id = _nextId++;
            _customers.Add(customer);
            _logger.LogInfo($"Dodano klienta: {customer.FullName}");
        }

        public IEnumerable<Customer> GetAllCustomers() => _customers;
        public Customer GetCustomerById(int id) => _customers.FirstOrDefault(c => c.Id == id);
    }
}