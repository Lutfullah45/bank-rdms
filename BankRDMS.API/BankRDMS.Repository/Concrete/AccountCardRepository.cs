using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Repository.Concrete;

public class AccountCardRepository : IAccountCardRepository
{
    private readonly AppDbContext _context;

    public AccountCardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AccountCard accountCard)
    {
        await _context.AccountCards.AddAsync(accountCard);
        await _context.SaveChangesAsync();
    }

    public AccountCard? GetById(int id, int customerId)
    {
        return _context.AccountCards
            .AsNoTracking()
            .FirstOrDefault(card => card.Id == id && card.Account.CustomerId == customerId);
    }

    public Task<AccountCard?> GetActiveByCardNumberAsync(string cardNumber, int customerId)
    {
        return _context.AccountCards
            .FirstOrDefaultAsync(card =>
                card.CardNumber == cardNumber &&
                card.IsActive &&
                card.Account.IsActive &&
                card.Account.CustomerId == customerId);
    }

    public Task<List<AccountCard>> GetAllAsync(int customerId)
    {
        return _context.AccountCards
            .AsNoTracking()
            .Where(card => card.Account.CustomerId == customerId)
            .ToListAsync();
    }

    public List<AccountCard> GetByAccountId(int accountId, int customerId)
    {
        return _context.AccountCards
            .AsNoTracking()
            .Where(card => card.AccountId == accountId && card.Account.CustomerId == customerId)
            .ToList();
    }

    public async Task<bool> UpdateStatusAsync(int id, int customerId, bool isActive)
    {
        var card = await _context.AccountCards
            .FirstOrDefaultAsync(item => item.Id == id && item.Account.CustomerId == customerId);

        if (card is null)
        {
            return false;
        }

        card.IsActive = isActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> CardNumberExistsAsync(string cardNumber)
    {
        return _context.AccountCards.AnyAsync(card => card.CardNumber == cardNumber);
    }
}
