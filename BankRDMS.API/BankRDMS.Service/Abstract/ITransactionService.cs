using BankRDMS.Core.Entities;

namespace BankRDMS.Service.Abstract;

public interface ITransactionService
{
    Task DepositAsync(string cardNumber, decimal amount, string description);
    Task WithdrawAsync(string cardNumber, decimal amount, string description);
    Task PurchaseAsync(string creditCardNumber, decimal amount, string description);
    Task TransferToCreditCardAsync(
        string accountCardNumber,
        string creditCardNumber,
        decimal amount,
        string description);
    Task<List<Transaction>> GetAllTransactionsAsync();
    Transaction? GetTransactionById(int id);
}
