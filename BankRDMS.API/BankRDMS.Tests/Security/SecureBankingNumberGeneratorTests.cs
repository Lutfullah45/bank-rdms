using BankRDMS.Service.Concrete;

namespace BankRDMS.Tests.Security;

public class SecureBankingNumberGeneratorTests
{
    private readonly SecureBankingNumberGenerator _generator = new();

    [Fact]
    public void GenerateAccountNumber_ReturnsTenDigits()
    {
        var value = _generator.GenerateAccountNumber();

        Assert.Equal(10, value.Length);
        Assert.All(value, character => Assert.True(char.IsDigit(character)));
    }

    [Fact]
    public void GenerateIban_ReturnsValidLengthAndPrefix()
    {
        var value = _generator.GenerateIban();

        Assert.Equal(26, value.Length);
        Assert.StartsWith("TR", value);
        Assert.All(value[2..], character => Assert.True(char.IsDigit(character)));
    }

    [Fact]
    public void GenerateCardNumber_ReturnsSixteenDigits()
    {
        var value = _generator.GenerateCardNumber();

        Assert.Equal(16, value.Length);
        Assert.StartsWith("4532", value);
        Assert.All(value, character => Assert.True(char.IsDigit(character)));
    }

    [Fact]
    public void GenerateSecurityCode_ReturnsThreeDigits()
    {
        var value = _generator.GenerateSecurityCode();

        Assert.Equal(3, value.Length);
        Assert.All(value, character => Assert.True(char.IsDigit(character)));
    }

    [Fact]
    public void GeneratedCardNumbers_AreUniqueInSample()
    {
        var values = Enumerable.Range(0, 1_000)
            .Select(_ => _generator.GenerateCardNumber())
            .ToHashSet();

        Assert.Equal(1_000, values.Count);
    }
}
