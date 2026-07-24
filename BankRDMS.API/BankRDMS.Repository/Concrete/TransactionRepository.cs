using BankRDMS.Core.Constants;
using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Repository.Concrete;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;

    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public Task<decimal> GetAccountBalanceAsync(int accountId)
    {
        return _context.Transactions
            .Where(transaction => transaction.AccountId == accountId)
            .SumAsync(transaction =>
                transaction.TransactionTypeId == TransactionTypeIds.Deposit
                    ? transaction.Amount
                    : transaction.TransactionTypeId == TransactionTypeIds.Withdrawal ||
                      transaction.TransactionTypeId == TransactionTypeIds.CreditCardPayment
                        ? -transaction.Amount
                        : 0);
    }

    public Task<decimal> GetCreditCardDebtAsync(int creditCardId)
    {
        return _context.Transactions
            .Where(transaction => transaction.CreditCardId == creditCardId)
            .SumAsync(transaction =>
                transaction.TransactionTypeId == TransactionTypeIds.Purchase
                    ? transaction.Amount
                    : transaction.TransactionTypeId == TransactionTypeIds.CreditCardPayment
                        ? -transaction.Amount
                        : 0);
    }

    public Task<List<Transaction>> GetAllWithDetailsAsync(int customerId)
    {
        return _context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Account)
            .Include(transaction => transaction.CreditCard)
            .Include(transaction => transaction.TransactionType)
            .Where(transaction =>
                (transaction.Account != null && transaction.Account.CustomerId == customerId) ||
                (transaction.CreditCard != null && transaction.CreditCard.CustomerId == customerId))
            .ToListAsync();
    }

    public Transaction? GetByIdWithDetails(int id, int customerId)
    {
        return _context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Account)
            .Include(transaction => transaction.CreditCard)
            .Include(transaction => transaction.TransactionType)
            .FirstOrDefault(transaction =>
                transaction.Id == id &&
                ((transaction.Account != null && transaction.Account.CustomerId == customerId) ||
                 (transaction.CreditCard != null && transaction.CreditCard.CustomerId == customerId)));
    }
}
