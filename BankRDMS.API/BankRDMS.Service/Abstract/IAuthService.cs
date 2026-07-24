using BankRDMS.Core.Entities;

namespace BankRDMS.Service.Abstract;

public interface IAuthService
{
    Task<Customer?> AuthenticateAsync(string identityNumber, DateTime birthDate);
}
