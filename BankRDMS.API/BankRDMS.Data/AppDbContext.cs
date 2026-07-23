using BankRDMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountCard> AccountCards { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}