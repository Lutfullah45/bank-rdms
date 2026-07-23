namespace BankRDMS.Core.Entities;

public class Transaction
{
    public int Id { get; set; }
    public int? AccountId { get; set; } 
    public int? CreditCardId { get; set; } 
    public int TransactionTypeId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Account? Account { get; set; }
    public CreditCard? CreditCard { get; set; }
    public TransactionType TransactionType { get; set; } = null!;
}