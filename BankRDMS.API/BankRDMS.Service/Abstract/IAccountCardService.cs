using BankRDMS.Core.Entities;

namespace BankRDMS.Service.Abstract
{
    public interface IAccountCardService
    {
        Task CreateAccountCardAsync(int accountId);
        Task<List<AccountCard>> GetAllAccountCardsAsync();
        AccountCard GetAccountCardById(int id);
        List<AccountCard> GetAccountCardsByAccountId(int accountId);
        Task<bool> UpdateStatusAsync(int id, bool isActive);
    }
}