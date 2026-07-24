using BankRDMS.Core.Entities;
using BankRDMS.Core.Exceptions;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;

namespace BankRDMS.Service.Concrete;

public class CreditCardManager : ICreditCardService
{
    private readonly IAccountCardRepository _accountCardRepository;
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentCustomer _currentCustomer;
    private readonly IBankingNumberGenerator _numberGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public CreditCardManager(
        IAccountCardRepository accountCardRepository,
        ICreditCardRepository creditCardRepository,
        ICustomerRepository customerRepository,
        ICurrentCustomer currentCustomer,
        IBankingNumberGenerator numberGenerator,
        IUnitOfWork unitOfWork)
    {
        _accountCardRepository = accountCardRepository;
        _creditCardRepository = creditCardRepository;
        _customerRepository = customerRepository;
        _currentCustomer = currentCustomer;
        _numberGenerator = numberGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateCreditCardAsync(int customerId, decimal requestedLimit)
    {
        if (customerId != _currentCustomer.CustomerId)
        {
            throw new ForbiddenException("Başka bir müşteri adına kredi kartı oluşturamazsınız.");
        }

        if (requestedLimit <= 0)
        {
            throw new DomainValidationException("Kart limiti sıfırdan büyük olmalıdır.");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var customer = await _customerRepository.GetByIdForUpdateAsync(customerId);
            if (customer is null)
            {
                throw new EntityNotFoundException("Müşteri bulunamadı.");
            }

            var currentTotalLimit = await _creditCardRepository.GetTotalActiveLimitAsync(customerId);
            if (currentTotalLimit + requestedLimit > customer.RiskLimit)
            {
                throw new DomainValidationException(
                    "Aktif kredi kartı limitleri toplamı müşteri risk limitini aşamaz.");
            }

            var newCreditCard = new CreditCard
            {
                CustomerId = customerId,
                CardNumber = await GenerateUniqueCardNumberAsync(),
                CardLimit = requestedLimit,
                ExpirationMonthYear = DateTime.UtcNow.AddYears(5).ToString("MM/yy"),
                Ccv = _numberGenerator.GenerateSecurityCode(),
                IsActive = true
            };

            await _creditCardRepository.AddAsync(newCreditCard);
        });
    }

    public Task<List<CreditCard>> GetAllCreditCardsAsync()
    {
        return _creditCardRepository.GetAllAsync(_currentCustomer.CustomerId);
    }

    public CreditCard? GetCreditCardById(int id)
    {
        return _creditCardRepository.GetById(id, _currentCustomer.CustomerId);
    }

    public Task<List<CreditCard>> GetCreditCardsByCustomerIdAsync(int customerId)
    {
        if (customerId != _currentCustomer.CustomerId)
        {
            throw new ForbiddenException("Başka bir müşterinin kredi kartlarını görüntüleyemezsiniz.");
        }

        return _creditCardRepository.GetByCustomerIdAsync(customerId);
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
