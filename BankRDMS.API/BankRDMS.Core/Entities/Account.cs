namespace BankRDMS.Core.Entities;

public class Account
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Customer Customer { get; set; } = null!;
    public ICollection<AccountCard> AccountCards { get; set; } = new List<AccountCard>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}