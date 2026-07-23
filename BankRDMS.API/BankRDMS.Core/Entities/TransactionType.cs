namespace BankRDMS.Core.Entities;

public class TransactionType
{
    public int Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}