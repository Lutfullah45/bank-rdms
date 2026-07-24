using System.Security.Cryptography;
using System.Text;
using BankRDMS.Service.Abstract;

namespace BankRDMS.Service.Concrete;

public sealed class SecureBankingNumberGenerator : IBankingNumberGenerator
{
    public string GenerateAccountNumber()
    {
        return GenerateDigits(10);
    }

    public string GenerateIban()
    {
        return "TR" + GenerateDigits(24);
    }

    public string GenerateCardNumber()
    {
        return "4532" + GenerateDigits(12);
    }

    public string GenerateSecurityCode()
    {
        return GenerateDigits(3);
    }

    private static string GenerateDigits(int length)
    {
        var builder = new StringBuilder(length);

        for (var index = 0; index < length; index++)
        {
            builder.Append(RandomNumberGenerator.GetInt32(10));
        }

        return builder.ToString();
    }
}
