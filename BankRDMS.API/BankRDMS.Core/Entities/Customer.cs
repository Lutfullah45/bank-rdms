namespace BankRDMS.Core.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public string BirthPlace { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public decimal RiskLimit { get; set; } = 10000.00m; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
}