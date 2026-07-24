using BankRDMS.Core.Entities;
using BankRDMS.Core.Exceptions;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;

namespace BankRDMS.Service.Concrete;

public class CustomerManager : ICustomerService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountCardRepository _accountCardRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICurrentCustomer _currentCustomer;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBankingNumberGenerator _numberGenerator;

    public CustomerManager(
        IAccountRepository accountRepository,
        IAccountCardRepository accountCardRepository,
        ICreditCardRepository creditCardRepository,
        ICurrentCustomer currentCustomer,
        ICustomerRepository customerRepository,
        IBankingNumberGenerator numberGenerator)
    {
        _accountRepository = accountRepository;
        _accountCardRepository = accountCardRepository;
        _creditCardRepository = creditCardRepository;
        _currentCustomer = currentCustomer;
        _customerRepository = customerRepository;
        _numberGenerator = numberGenerator;
    }

    public async Task AddCustomerWithAccountAsync(Customer customer)
    {
        ValidateCustomer(customer);

        customer.IdentityNumber = customer.IdentityNumber.Trim();
        customer.FullName = customer.FullName.Trim();
        customer.BirthPlace = customer.BirthPlace.Trim();
        customer.CreatedAt = DateTime.UtcNow;
        customer.RiskLimit = 10_000m;

        if (await _customerRepository.IdentityNumberExistsAsync(customer.IdentityNumber))
        {
            throw new ConflictException("Bu kimlik numarasıyla daha önce müşteri oluşturulmuş.");
        }

        var newAccount = new Account
        {
            AccountName = "Vadesiz Hesap",
            AccountNumber = await GenerateUniqueAccountNumberAsync(),
            Iban = await GenerateUniqueIbanAsync(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            AccountCards = new List<AccountCard>
            {
                new()
                {
                    CardNumber = await GenerateUniqueCardNumberAsync(),
                    ExpirationMonthYear = DateTime.UtcNow.AddYears(5).ToString("MM/yy"),
                    Ccv = _numberGenerator.GenerateSecurityCode(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        customer.Accounts = new List<Account> { newAccount };
        await _customerRepository.AddAsync(customer);
    }

    public List<Customer> GetAllCustomers()
    {
        var customer = _customerRepository.GetByIdWithDetails(_currentCustomer.CustomerId);
        return customer is null ? [] : [customer];
    }

    public Customer? GetCustomerById(int id)
    {
        if (id != _currentCustomer.CustomerId)
        {
            return null;
        }

        return _customerRepository.GetByIdWithDetails(id);
    }

    private static void ValidateCustomer(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FullName) || customer.FullName.Trim().Length is < 2 or > 100)
        {
            throw new DomainValidationException("Ad soyad 2 ile 100 karakter arasında olmalıdır.");
        }

        if (customer.IdentityNumber.Length != 11 || !customer.IdentityNumber.All(char.IsDigit))
        {
            throw new DomainValidationException("Kimlik numarası 11 rakamdan oluşmalıdır.");
        }

        if (string.IsNullOrWhiteSpace(customer.BirthPlace) || customer.BirthPlace.Trim().Length > 50)
        {
            throw new DomainValidationException("Doğum yeri zorunludur ve en fazla 50 karakter olabilir.");
        }

        if (customer.BirthDate.Date > DateTime.UtcNow.Date.AddYears(-18))
        {
            throw new DomainValidationException("Müşteri en az 18 yaşında olmalıdır.");
        }

    }

    private async Task<string> GenerateUniqueAccountNumberAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var value = _numberGenerator.GenerateAccountNumber();
            if (!await _accountRepository.AccountNumberExistsAsync(value))
            {
                return value;
            }
        }

        throw new ConflictException("Benzersiz hesap numarası üretilemedi.");
    }

    private async Task<string> GenerateUniqueIbanAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var value = _numberGenerator.GenerateIban();
            if (!await _accountRepository.IbanExistsAsync(value))
            {
                return value;
            }
        }

        throw new ConflictException("Benzersiz IBAN üretilemedi.");
    }

    private async Task<string> GenerateUniqueCardNumberAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var value = _numberGenerator.GenerateCardNumber();
            if (!await _accountCardRepository.CardNumberExistsAsync(value) &&
                !await _creditCardRepository.CardNumberExistsAsync(value))
            {
                return value;
            }
        }

        throw new ConflictException("Benzersiz kart numarası üretilemedi.");
    }
}
