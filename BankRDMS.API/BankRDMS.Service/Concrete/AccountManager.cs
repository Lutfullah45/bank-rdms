using BankApi.Services;
using BankRDMS.Core.Entities;
using BankRDMS.Core.Exceptions;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;

namespace BankRDMS.Service.Concrete;

public class AccountManager : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentCustomer _currentCustomer;
    private readonly IBankingNumberGenerator _numberGenerator;

    public AccountManager(
        IAccountRepository accountRepository,
        ICurrentCustomer currentCustomer,
        IBankingNumberGenerator numberGenerator)
    {
        _accountRepository = accountRepository;
        _currentCustomer = currentCustomer;
        _numberGenerator = numberGenerator;
    }

    public async Task CreateAccountAsync(int customerId, string accountName)
    {
        EnsureCurrentCustomer(customerId);

        if (string.IsNullOrWhiteSpace(accountName) || accountName.Trim().Length is < 2 or > 50)
        {
            throw new DomainValidationException("Hesap adı 2 ile 50 karakter arasında olmalıdır.");
        }

        var newAccount = new Account
        {
            CustomerId = customerId,
            AccountName = accountName.Trim(),
            IsActive = true,
            Iban = await GenerateUniqueIbanAsync(),
            AccountNumber = await GenerateUniqueAccountNumberAsync()
        };

        await _accountRepository.AddAsync(newAccount);
    }

    public Task<List<Account>> GetAllAccountsAsync()
    {
        return _accountRepository.GetAllByCustomerIdAsync(_currentCustomer.CustomerId);
    }

    public Account? GetAccountById(int id)
    {
        return _accountRepository.GetByIdWithCards(id, _currentCustomer.CustomerId);
    }

    public Task<List<Account>> GetAccountsByCustomerIdAsync(int customerId)
    {
        EnsureCurrentCustomer(customerId);
        return _accountRepository.GetByCustomerIdAsync(customerId);
    }

    public Task<bool> UpdateAccountStatusAsync(int id, bool isActive)
    {
        return _accountRepository.UpdateStatusAsync(id, _currentCustomer.CustomerId, isActive);
    }

    private void EnsureCurrentCustomer(int customerId)
    {
        if (customerId != _currentCustomer.CustomerId)
        {
            throw new ForbiddenException("Başka bir müşterinin hesabında işlem yapamazsınız.");
        }
    }

    private async Task<string> GenerateUniqueAccountNumberAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var accountNumber = _numberGenerator.GenerateAccountNumber();
            if (!await _accountRepository.AccountNumberExistsAsync(accountNumber))
            {
                return accountNumber;
            }
        }

        throw new ConflictException("Benzersiz hesap numarası üretilemedi. Lütfen tekrar deneyin.");
    }

    private async Task<string> GenerateUniqueIbanAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var iban = _numberGenerator.GenerateIban();
            if (!await _accountRepository.IbanExistsAsync(iban))
            {
                return iban;
            }
        }

        throw new ConflictException("Benzersiz IBAN üretilemedi. Lütfen tekrar deneyin.");
    }
}
