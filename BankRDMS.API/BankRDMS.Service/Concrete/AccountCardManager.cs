using BankRDMS.Core.Entities;
using BankRDMS.Core.Exceptions;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;

namespace BankRDMS.Service.Concrete;

public class AccountCardManager : IAccountCardService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountCardRepository _accountCardRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICurrentCustomer _currentCustomer;
    private readonly IBankingNumberGenerator _numberGenerator;

    public AccountCardManager(
        IAccountRepository accountRepository,
        IAccountCardRepository accountCardRepository,
        ICreditCardRepository creditCardRepository,
        ICurrentCustomer currentCustomer,
        IBankingNumberGenerator numberGenerator)
    {
        _accountRepository = accountRepository;
        _accountCardRepository = accountCardRepository;
        _creditCardRepository = creditCardRepository;
        _currentCustomer = currentCustomer;
        _numberGenerator = numberGenerator;
    }

    public async Task CreateAccountCardAsync(int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, _currentCustomer.CustomerId);
        if (account is null)
        {
            throw new EntityNotFoundException("Hesap bulunamadı.");
        }

        if (!account.IsActive)
        {
            throw new DomainValidationException("Pasif hesaba kart oluşturulamaz.");
        }

        var newAccountCard = new AccountCard
        {
            AccountId = accountId,
            CardNumber = await GenerateUniqueCardNumberAsync(),
            ExpirationMonthYear = DateTime.UtcNow.AddYears(3).ToString("MM/yy"),
            Ccv = _numberGenerator.GenerateSecurityCode(),
            IsActive = true
        };

        await _accountCardRepository.AddAsync(newAccountCard);
    }

    public AccountCard? GetAccountCardById(int id)
    {
        return _accountCardRepository.GetById(id, _currentCustomer.CustomerId);
    }

    public Task<bool> UpdateStatusAsync(int id, bool isActive)
    {
        return _accountCardRepository.UpdateStatusAsync(id, _currentCustomer.CustomerId, isActive);
    }

    public Task<List<AccountCard>> GetAllAccountCardsAsync()
    {
        return _accountCardRepository.GetAllAsync(_currentCustomer.CustomerId);
    }

    public List<AccountCard> GetAccountCardsByAccountId(int accountId)
    {
        return _accountCardRepository.GetByAccountId(accountId, _currentCustomer.CustomerId);
    }

    private async Task<string> GenerateUniqueCardNumberAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var cardNumber = _numberGenerator.GenerateCardNumber();
            var existsAsAccountCard = await _accountCardRepository.CardNumberExistsAsync(cardNumber);
            var existsAsCreditCard = await _creditCardRepository.CardNumberExistsAsync(cardNumber);

            if (!existsAsAccountCard && !existsAsCreditCard)
            {
                return cardNumber;
            }
        }

        throw new ConflictException("Benzersiz kart numarası üretilemedi. Lütfen tekrar deneyin.");
    }
}
