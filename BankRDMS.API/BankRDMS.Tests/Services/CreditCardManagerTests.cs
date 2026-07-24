using BankRDMS.Core.Entities;
using BankRDMS.Core.Exceptions;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;
using BankRDMS.Service.Concrete;

namespace BankRDMS.Tests.Services;

public class CreditCardManagerTests
{
    [Fact]
    public async Task CreateCreditCard_UsesRequestedLimit()
    {
        var creditCardRepository = new StubCreditCardRepository(currentTotalLimit: 2_000m);
        var manager = CreateManager(creditCardRepository, riskLimit: 10_000m);

        await manager.CreateCreditCardAsync(3, 2_500m);

        Assert.NotNull(creditCardRepository.AddedCard);
        Assert.Equal(2_500m, creditCardRepository.AddedCard.CardLimit);
    }

    [Fact]
    public async Task CreateCreditCard_RejectsRiskLimitOverflow()
    {
        var creditCardRepository = new StubCreditCardRepository(currentTotalLimit: 9_000m);
        var manager = CreateManager(creditCardRepository, riskLimit: 10_000m);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => manager.CreateCreditCardAsync(3, 2_500m));
    }

    private static CreditCardManager CreateManager(
        StubCreditCardRepository creditCardRepository,
        decimal riskLimit)
    {
        return new CreditCardManager(
            new StubAccountCardRepository(),
            creditCardRepository,
            new StubCustomerRepository(riskLimit),
            new StubCurrentCustomer(),
            new StubNumberGenerator(),
            new ImmediateUnitOfWork());
    }

    private sealed class StubCurrentCustomer : ICurrentCustomer
    {
        public int CustomerId => 3;
    }

    private sealed class StubNumberGenerator : IBankingNumberGenerator
    {
        public string GenerateAccountNumber() => "1234567890";
        public string GenerateIban() => "TR123456789012345678901234";
        public string GenerateCardNumber() => "4532123412341234";
        public string GenerateSecurityCode() => "123";
    }

    private sealed class ImmediateUnitOfWork : IUnitOfWork
    {
        public Task ExecuteInTransactionAsync(
            Func<Task> operation,
            CancellationToken cancellationToken = default) => operation();
    }

    private sealed class StubCustomerRepository(decimal riskLimit) : ICustomerRepository
    {
        public Task<Customer?> GetByIdForUpdateAsync(int id) =>
            Task.FromResult<Customer?>(new Customer { Id = id, RiskLimit = riskLimit });

        public Task AddAsync(Customer customer) => Task.CompletedTask;
        public Customer? GetByIdWithDetails(int id) => null;
        public Task<Customer?> GetByIdentityNumberAsync(string identityNumber) => Task.FromResult<Customer?>(null);
        public Task<bool> IdentityNumberExistsAsync(string identityNumber) => Task.FromResult(false);
    }

    private sealed class StubCreditCardRepository(decimal currentTotalLimit) : ICreditCardRepository
    {
        public CreditCard? AddedCard { get; private set; }

        public Task AddAsync(CreditCard creditCard)
        {
            AddedCard = creditCard;
            return Task.CompletedTask;
        }

        public Task<decimal> GetTotalActiveLimitAsync(int customerId) =>
            Task.FromResult(currentTotalLimit);

        public Task<bool> CardNumberExistsAsync(string cardNumber) => Task.FromResult(false);
        public Task<List<CreditCard>> GetAllAsync(int customerId) => Task.FromResult(new List<CreditCard>());
        public CreditCard? GetById(int id, int customerId) => null;
        public Task<CreditCard?> GetActiveByCardNumberForUpdateAsync(string cardNumber, int customerId) =>
            Task.FromResult<CreditCard?>(null);
        public Task<List<CreditCard>> GetByCustomerIdAsync(int customerId) =>
            Task.FromResult(new List<CreditCard>());
    }

    private sealed class StubAccountCardRepository : IAccountCardRepository
    {
        public Task<bool> CardNumberExistsAsync(string cardNumber) => Task.FromResult(false);
        public Task AddAsync(AccountCard accountCard) => Task.CompletedTask;
        public AccountCard? GetById(int id, int customerId) => null;
        public Task<AccountCard?> GetActiveByCardNumberAsync(string cardNumber, int customerId) =>
            Task.FromResult<AccountCard?>(null);
        public Task<List<AccountCard>> GetAllAsync(int customerId) => Task.FromResult(new List<AccountCard>());
        public List<AccountCard> GetByAccountId(int accountId, int customerId) => [];
        public Task<bool> UpdateStatusAsync(int id, int customerId, bool isActive) => Task.FromResult(false);
    }
}
