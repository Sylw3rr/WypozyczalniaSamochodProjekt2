using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SQLite;       // dla SQLiteConnection, SQLiteCommand
using CarRentalSystem.Models;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Utils;
using System.Windows.Forms;

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
        }

        public void AddCustomer(Customer customer)
        {
            customer.Id = _nextId++;
            _customers.Add(customer);
            SaveCustomerToDb(customer);
            _logger.LogInfo($"Dodano klienta: {customer.FullName}");
        }

        public IEnumerable<Customer> GetAllCustomers() => _customers;
        public Customer GetCustomerById(int id) => _customers.FirstOrDefault(c => c.Id == id);
        public void SaveCustomerToDb(Customer customer)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("INSERT OR REPLACE INTO Customers (Id, FirstName, LastName, Email, DateOfBirth) VALUES (@Id, @FirstName, @LastName, @Email, @DateOfBirth)", conn);
                cmd.Parameters.AddWithValue("@Id", customer.Id);
                cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@DateOfBirth", customer.DateOfBirth.ToString("yyyy-MM-dd")); // Standardowy format daty
                cmd.ExecuteNonQuery();
            }
        }

        // Metoda do wczytania wszystkich klientów z bazy przy starcie aplikacji
        public void LoadCustomersFromDb()
        {
            _customers.Clear();
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("SELECT * FROM Customers", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var customer = new Customer(
                            id: Convert.ToInt32(reader["Id"]),
                            firstName: reader["FirstName"].ToString(),
                            lastName: reader["LastName"].ToString(),
                            email: reader["Email"].ToString(), // ✅ DODANO
                            phone: reader["PhoneNumber"]?.ToString() ?? "", // ✅ DODANO
                            dob: DateTime.Parse(reader["DateOfBirth"].ToString())
                        );
                        _customers.Add(customer);
                    }
                }
            }
            if (_customers.Any())
            {
                _nextId = _customers.Max(c => c.Id) + 1;
            }
        }
        public void UpdateCustomer(Customer customer)
        {
            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.Email = customer.Email;
                existingCustomer.PhoneNumber = customer.PhoneNumber;
                existingCustomer.DateOfBirth = customer.DateOfBirth;

                SaveCustomerToDb(existingCustomer);
                _logger.LogInfo($"Zaktualizowano klienta: {existingCustomer.FullName}");
            }
        }

        public void DeleteCustomer(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                DeleteCustomerFromDb(id);
                _logger.LogInfo($"Usunięto klienta: {customer.FullName}");
            }
        }

        public void DeleteCustomerFromDb(int id)
        {
            using (var conn = Database.GetConnection())
            {
                var cmd = new SQLiteCommand("DELETE FROM Customers WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}