using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Service.Concrete
{
    public class CreditCardManager : ICreditCardService
    {
        private readonly AppDbContext _context;

        public CreditCardManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateCreditCardAsync(int customerId, decimal requestedLimit)
        {
            var random = new Random();

            string cardNumber = $"4532{random.Next(1000, 9999)}{random.Next(1000, 9999)}{random.Next(1000, 9999)}";

            var newCreditCard = new CreditCard
            {
                CustomerId = customerId,
                CardNumber = cardNumber,
                CardLimit = 10000,
                ExpirationMonthYear = DateTime.Now.AddYears(5).ToString("MM/yy"), 
                Ccv = random.Next(100, 999).ToString(),
                IsActive = true
            };

            await _context.CreditCards.AddAsync(newCreditCard);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CreditCard>> GetAllCreditCardsAsync()
        {
            return await _context.CreditCards.ToListAsync();
        }
        public CreditCard GetCreditCardById(int id)
        {
            return _context.CreditCards.FirstOrDefault(cc => cc.Id == id);
        }

        public async Task<List<CreditCard>> GetCreditCardsByCustomerIdAsync(int customerId)
        {
            return await _context.CreditCards.Where(c => c.CustomerId == customerId).ToListAsync();
        }
    }
}