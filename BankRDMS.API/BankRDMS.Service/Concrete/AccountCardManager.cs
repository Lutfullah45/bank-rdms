using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Service.Concrete
{
    public class AccountCardManager : IAccountCardService
    {
        private readonly AppDbContext _context;

        public AccountCardManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAccountCardAsync(int accountId)
        {
            var random = new Random();

            // Banka kartı için rastgele 16 haneli numara üretimi
            string cardNumber = $"4532{random.Next(1000, 9999)}{random.Next(1000, 9999)}{random.Next(1000, 9999)}";

            var existingAccount = await _context.Accounts.FindAsync(accountId);

            if (existingAccount == null)
            {
                throw new Exception($"{accountId} ID'li hesap bulunamadığı için kart oluşturulamaz.");
               
            }

            var newAccountCard = new AccountCard
            {
                AccountId = accountId,
                CardNumber = cardNumber,
                ExpirationMonthYear = DateTime.Now.AddYears(3).ToString("MM/yy"),
                Ccv = random.Next(100, 999).ToString(),
                IsActive = true
            };

            await _context.AccountCards.AddAsync(newAccountCard);
            await _context.SaveChangesAsync();
        }
        public AccountCard GetAccountCardById(int id)
        {
            return _context.AccountCards.FirstOrDefault(ac => ac.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(int id, bool isActive)
        {
        
            var card = await _context.AccountCards.FindAsync(id);

            if (card == null)
            {
                return false; 
            }

            card.IsActive = isActive;
            _context.AccountCards.Update(card);
            await _context.SaveChangesAsync();

            return true; 
        }

        public async Task<List<AccountCard>> GetAllAccountCardsAsync()
        {
            return await _context.AccountCards.ToListAsync();
        }
        public List<AccountCard> GetAccountCardsByAccountId(int accountId)
        {
            return _context.AccountCards
                           .Where(ac => ac.AccountId == accountId)
                           .ToList();
        }
    }
}