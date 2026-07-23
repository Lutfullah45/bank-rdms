namespace BankRDMS.Core.Entities;

public class AccountCard
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string ExpirationMonthYear { get; set; } = string.Empty;
    public string Ccv { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Account Account { get; set; } = null!;
}