using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
        void AddCustomer(Customer customer);
    }
}