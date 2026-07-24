using BankRDMS.Core.Entities;
using BankRDMS.Repository.Abstract;
using BankRDMS.Service.Abstract;

namespace BankRDMS.Service.Concrete;

public sealed class AuthManager : IAuthService
{
    private readonly ICustomerRepository _customerRepository;

    public AuthManager(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer?> AuthenticateAsync(string identityNumber, DateTime birthDate)
    {
        var customer = await _customerRepository.GetByIdentityNumberAsync(identityNumber.Trim());

        return customer is not null && customer.BirthDate.Date == birthDate.Date
            ? customer
            : null;
    }
}
