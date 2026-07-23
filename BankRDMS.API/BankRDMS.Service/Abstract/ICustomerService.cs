using BankRDMS.Core.Entities;
using System.Collections.Generic;

namespace BankRDMS.Service.Abstract
{
    public interface ICustomerService
    {
        bool AddCustomerWithAccount(Customer customer);
        List<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
    }
}