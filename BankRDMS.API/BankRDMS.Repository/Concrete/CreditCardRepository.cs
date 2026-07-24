using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Repository.Concrete;

public class CreditCardRepository : ICreditCardRepository
{
    private readonly AppDbContext _context;

    public CreditCardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CreditCard creditCard)
    {
        await _context.CreditCards.AddAsync(creditCard);
        await _context.SaveChangesAsync();
    }

    public Task<List<CreditCard>> GetAllAsync(int customerId)
    {
        return _context.CreditCards
            .AsNoTracking()
            .Where(card => card.CustomerId == customerId)
            .ToListAsync();
    }

    public CreditCard? GetById(int id, int customerId)
    {
        return _context.CreditCards
            .AsNoTracking()
            .FirstOrDefault(card => card.Id == id && card.CustomerId == customerId);
    }

    public Task<CreditCard?> GetActiveByCardNumberForUpdateAsync(string cardNumber, int customerId)
    {
        return _context.CreditCards
            .FromSqlInterpolated(
                $"SELECT * FROM credit_cards WHERE card_number = {cardNumber} AND customer_id = {customerId} AND is_active = TRUE FOR UPDATE")
            .SingleOrDefaultAsync();
    }

    public Task<List<CreditCard>> GetByCustomerIdAsync(int customerId)
    {
        return _context.CreditCards
            .AsNoTracking()
            .Where(card => card.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalActiveLimitAsync(int customerId)
    {
        return await _context.CreditCards
            .Where(card => card.CustomerId == customerId && card.IsActive)
            .SumAsync(card => card.CardLimit);
    }

    public Task<bool> CardNumberExistsAsync(string cardNumber)
    {
        return _context.CreditCards.AnyAsync(card => card.CardNumber == cardNumber);
    }
}
