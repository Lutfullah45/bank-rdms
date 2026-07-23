using System.Transactions;

namespace BankRDMS.Core.Entities;

public class CreditCard
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string ExpirationMonthYear { get; set; } = string.Empty;
    public string Ccv { get; set; } = string.Empty;
    public decimal CardLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Customer Customer { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}