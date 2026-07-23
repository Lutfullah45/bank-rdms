using System.Threading.Tasks;
using BankRDMS.Core.Entities;

namespace BankApi.Services
{
    public interface IAccountService
    {
        Task CreateAccountAsync(int customerId, string accountName);
        Task<List<Account>> GetAllAccountsAsync();
        Account GetAccountById(int id);
        Task<List<Account>> GetAccountsByCustomerIdAsync(int customerId);
        bool UpdateAccountStatus(int id, bool isActive);
    }
}