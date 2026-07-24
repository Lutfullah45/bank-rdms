namespace BankRDMS.Service.Abstract;

public interface IBankingNumberGenerator
{
    string GenerateAccountNumber();
    string GenerateIban();
    string GenerateCardNumber();
    string GenerateSecurityCode();
}
