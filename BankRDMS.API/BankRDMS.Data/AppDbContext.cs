using BankRDMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountCard> AccountCards => Set<AccountCard>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<TransactionType> TransactionTypes => Set<TransactionType>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(customer => customer.IdentityNumber).IsUnique();
            entity.Property(customer => customer.FullName).HasMaxLength(100).IsRequired();
            entity.Property(customer => customer.IdentityNumber).HasMaxLength(11).IsFixedLength().IsRequired();
            entity.Property(customer => customer.BirthPlace).HasMaxLength(50).IsRequired();
            entity.Property(customer => customer.RiskLimit).HasPrecision(15, 2);
            entity.ToTable(table => table.HasCheckConstraint(
                "ck_customers_risk_limit",
                "risk_limit >= 0"));
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(account => account.AccountNumber).IsUnique();
            entity.HasIndex(account => account.Iban).IsUnique();
            entity.Property(account => account.AccountName).HasMaxLength(50).IsRequired();
            entity.Property(account => account.AccountNumber).HasMaxLength(50).IsRequired();
            entity.Property(account => account.Iban).HasMaxLength(26).IsRequired();
        });

        modelBuilder.Entity<AccountCard>(entity =>
        {
            entity.HasIndex(card => card.CardNumber).IsUnique();
            entity.Property(card => card.CardNumber).HasMaxLength(16).IsFixedLength().IsRequired();
            entity.Property(card => card.ExpirationMonthYear).HasMaxLength(5).IsRequired();
            entity.Property(card => card.Ccv).HasMaxLength(3).IsRequired();
        });

        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasIndex(card => card.CardNumber).IsUnique();
            entity.Property(card => card.CardNumber).HasMaxLength(16).IsFixedLength().IsRequired();
            entity.Property(card => card.ExpirationMonthYear).HasMaxLength(5).IsRequired();
            entity.Property(card => card.Ccv).HasMaxLength(3).IsRequired();
            entity.Property(card => card.CardLimit).HasPrecision(15, 2);
            entity.ToTable(table => table.HasCheckConstraint(
                "ck_credit_cards_card_limit",
                "card_limit >= 0"));
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasIndex(transactionType => transactionType.TypeName).IsUnique();
            entity.Property(transactionType => transactionType.TypeName).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(transaction => transaction.Amount).HasPrecision(15, 2);
            entity.Property(transaction => transaction.Description).HasMaxLength(100);
            entity.ToTable(table => table.HasCheckConstraint(
                "ck_transactions_amount",
                "amount > 0"));
        });
    }
}
