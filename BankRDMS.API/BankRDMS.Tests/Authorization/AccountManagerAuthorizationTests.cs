using BankApi.Services;
using BankRDMS.Core.Entities;
using BankRDMS.Core.Exceptions;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;
using BankRDMS.Service.Concrete;

namespace BankRDMS.Tests.Authorization;

public class AccountManagerAuthorizationTests
{
    [Fact]
    public async Task CreateAccount_RejectsDifferentCustomerId()
    {
        var manager = CreateManager(currentCustomerId: 7);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => manager.CreateAccountAsync(8, "Vadesiz Hesap"));
    }

    [Fact]
    public async Task GetAllAccounts_UsesAuthenticatedCustomerId()
    {
        var repository = new StubAccountRepository();
        var manager = CreateManager(currentCustomerId: 7, repository);

        await manager.GetAllAccountsAsync();

        Assert.Equal(7, repository.LastRequestedCustomerId);
    }

    private static IAccountService CreateManager(
        int currentCustomerId,
        StubAccountRepository? repository = null)
    {
        return new AccountManager(
            repository ?? new StubAccountRepository(),
            new StubCurrentCustomer(currentCustomerId),
            new StubNumberGenerator());
    }

    private sealed class StubCurrentCustomer(int customerId) : ICurrentCustomer
    {
        public int CustomerId { get; } = customerId;
    }

    private sealed class StubNumberGenerator : IBankingNumberGenerator
    {
        public string GenerateAccountNumber() => "1234567890";
        public string GenerateIban() => "TR123456789012345678901234";
        public string GenerateCardNumber() => "4532123412341234";
        public string GenerateSecurityCode() => "123";
    }

    private sealed class StubAccountRepository : IAccountRepository
    {
        public int? LastRequestedCustomerId { get; private set; }

        public Task AddAsync(Account account) => Task.CompletedTask;

        public Task<List<Account>> GetAllByCustomerIdAsync(int customerId)
        {
            LastRequestedCustomerId = customerId;
            return Task.FromResult(new List<Account>());
        }

        public Account? GetByIdWithCards(int id, int customerId) => null;
        public Task<Account?> GetByIdAsync(int id, int customerId) => Task.FromResult<Account?>(null);
        public Task<Account?> GetByIdForUpdateAsync(int id, int customerId) => Task.FromResult<Account?>(null);
        public Task<List<Account>> GetByCustomerIdAsync(int customerId) => Task.FromResult(new List<Account>());
        public Task<bool> UpdateStatusAsync(int id, int customerId, bool isActive) => Task.FromResult(false);
        public Task<bool> AccountNumberExistsAsync(string accountNumber) => Task.FromResult(false);
        public Task<bool> IbanExistsAsync(string iban) => Task.FromResult(false);
    }
}
