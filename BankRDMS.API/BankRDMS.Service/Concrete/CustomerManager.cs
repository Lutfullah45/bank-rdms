using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Service.Concrete
{
    public class CustomerManager : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerManager(AppDbContext context)
        {
            _context = context;
        }

        public bool AddCustomerWithAccount(Customer customer)
        {
            try
            {
                customer.CreatedAt = DateTime.UtcNow;
                customer.RiskLimit = 10000;
                Account newAccount = new Account
                {
                    AccountName = "Vadesiz Hesap",
                    AccountNumber = GenerateUniqueAccountNumber(),
                    Iban = GenerateUniqueIban(),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    AccountCards = new List<AccountCard>
                    {
                        new AccountCard
                        {
                            CardNumber = GenerateCardNumber(),
                            Ccv = GenerateCvv(),
                            ExpirationMonthYear = GenerateExpireDate(),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow 
                        }
                    }
                };

                customer.Accounts = new List<Account> { newAccount };

                _context.Customers.Add(customer);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n=================== VERİTABANI HATASI ===================");
                Console.WriteLine("HATA: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("İÇ HATA (ASIL NEDEN): " + ex.InnerException.Message);
                }
                Console.WriteLine("=========================================================\n");
                throw;
            }
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers
                           .Include(c => c.Accounts) 
                               .ThenInclude(a => a.AccountCards) 
                           .Include(c => c.CreditCards) 
                           .ToList();
        }
        public Customer GetCustomerById(int id)
        {
            return _context.Customers
                           .Include(c => c.Accounts)
                               .ThenInclude(a => a.AccountCards)
                           .Include(c => c.CreditCards)
                           .FirstOrDefault(c => c.Id == id);
        }

        // --- Yardımcı Metotlar ---

        private string GenerateUniqueAccountNumber()
        {
            return new Random().Next(100000000, 999999999).ToString();
        }

        private string GenerateUniqueIban()
        {
            return "TR" + new Random().Next(10000000, 99999999).ToString() + "00000000000000";
        }

        private string GenerateCardNumber()
        {
            Random rnd = new Random();
            string firstPart = "40001234";
            string secondPart = rnd.Next(10000000, 99999999).ToString();
            return firstPart + secondPart;
        }

        private string GenerateCvv()
        {
            Random rnd = new Random();
            return rnd.Next(100, 999).ToString();
        }

        private string GenerateExpireDate()
        {
            DateTime expireDate = DateTime.UtcNow.AddYears(3);
            return expireDate.ToString("MM/yy");
        }
    }
}