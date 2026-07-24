using BankRDMS.Core.Constants;
using BankRDMS.Core.Entities;
using BankRDMS.Core.Exceptions;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;

namespace BankRDMS.Service.Concrete;

public class TransactionManager : ITransactionService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountCardRepository _accountCardRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICurrentCustomer _currentCustomer;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionManager(
        IAccountRepository accountRepository,
        IAccountCardRepository accountCardRepository,
        ICreditCardRepository creditCardRepository,
        ICurrentCustomer currentCustomer,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _accountCardRepository = accountCardRepository;
        _creditCardRepository = creditCardRepository;
        _currentCustomer = currentCustomer;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public Task DepositAsync(string cardNumber, decimal amount, string description)
    {
        ValidateTransactionInput(cardNumber, amount, description);

        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var accountCard = await GetActiveAccountCardAsync(cardNumber);
            var account = await LockActiveAccountAsync(accountCard.AccountId);

            await _transactionRepository.AddAsync(new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Description = description.Trim(),
                TransactionTypeId = TransactionTypeIds.Deposit
            });
        });
    }

    public Task WithdrawAsync(string cardNumber, decimal amount, string description)
    {
        ValidateTransactionInput(cardNumber, amount, description);

        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var accountCard = await GetActiveAccountCardAsync(cardNumber);
            var account = await LockActiveAccountAsync(accountCard.AccountId);
            var currentBalance = await _transactionRepository.GetAccountBalanceAsync(account.Id);

            if (currentBalance < amount)
            {
                throw new DomainValidationException("Yetersiz bakiye.");
            }

            await _transactionRepository.AddAsync(new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Description = description.Trim(),
                TransactionTypeId = TransactionTypeIds.Withdrawal
            });
        });
    }

    public Task PurchaseAsync(string creditCardNumber, decimal amount, string description)
    {
        ValidateTransactionInput(creditCardNumber, amount, description);

        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var creditCard = await _creditCardRepository.GetActiveByCardNumberForUpdateAsync(
                creditCardNumber,
                _currentCustomer.CustomerId);

            if (creditCard is null)
            {
                throw new EntityNotFoundException("Aktif kredi kartı bulunamadı.");
            }

            var currentDebt = await _transactionRepository.GetCreditCardDebtAsync(creditCard.Id);
            if (creditCard.CardLimit - currentDebt < amount)
            {
                throw new DomainValidationException("Yetersiz kredi kartı limiti.");
            }

            await _transactionRepository.AddAsync(new Transaction
            {
                CreditCardId = creditCard.Id,
                Amount = amount,
                Description = description.Trim(),
                TransactionTypeId = TransactionTypeIds.Purchase
            });
        });
    }

    public Task TransferToCreditCardAsync(
        string accountCardNumber,
        string creditCardNumber,
        decimal amount,
        string description)
    {
        ValidateTransactionInput(accountCardNumber, amount, description);

        if (string.IsNullOrWhiteSpace(creditCardNumber))
        {
            throw new DomainValidationException("Kredi kartı numarası zorunludur.");
        }

        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var accountCard = await GetActiveAccountCardAsync(accountCardNumber);
            var account = await LockActiveAccountAsync(accountCard.AccountId);
            var creditCard = await _creditCardRepository.GetActiveByCardNumberForUpdateAsync(
                creditCardNumber,
                _currentCustomer.CustomerId);

            if (creditCard is null)
            {
                throw new EntityNotFoundException("Aktif kredi kartı bulunamadı.");
            }

            var currentBalance = await _transactionRepository.GetAccountBalanceAsync(account.Id);
            if (currentBalance < amount)
            {
                throw new DomainValidationException("Yetersiz hesap bakiyesi.");
            }

            await _transactionRepository.AddAsync(new Transaction
            {
                AccountId = account.Id,
                CreditCardId = creditCard.Id,
                Amount = amount,
                Description = description.Trim(),
                TransactionTypeId = TransactionTypeIds.CreditCardPayment
            });
        });
    }

    public Task<List<Transaction>> GetAllTransactionsAsync()
    {
        return _transactionRepository.GetAllWithDetailsAsync(_currentCustomer.CustomerId);
    }

    public Transaction? GetTransactionById(int id)
    {
        return _transactionRepository.GetByIdWithDetails(id, _currentCustomer.CustomerId);
    }

    private async Task<AccountCard> GetActiveAccountCardAsync(string cardNumber)
    {
        return await _accountCardRepository.GetActiveByCardNumberAsync(
                   cardNumber,
                   _currentCustomer.CustomerId)
               ?? throw new EntityNotFoundException("Aktif banka kartı bulunamadı.");
    }

    private async Task<Account> LockActiveAccountAsync(int accountId)
    {
        var account = await _accountRepository.GetByIdForUpdateAsync(
            accountId,
            _currentCustomer.CustomerId);

        if (account is null || !account.IsActive)
        {
            throw new EntityNotFoundException("Aktif hesap bulunamadı.");
        }

        return account;
    }

    private static void ValidateTransactionInput(
        string cardNumber,
        decimal amount,
        string description)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            throw new DomainValidationException("Kart numarası zorunludur.");
        }

        if (amount <= 0)
        {
            throw new DomainValidationException("Tutar sıfırdan büyük olmalıdır.");
        }

        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length > 100)
        {
            throw new DomainValidationException("Açıklama zorunludur ve en fazla 100 karakter olabilir.");
        }
    }
}
