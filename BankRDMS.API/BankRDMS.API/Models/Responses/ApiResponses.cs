using BankRDMS.Core.Entities;

namespace BankRDMS.API.Models.Responses;

public sealed record AccountCardResponse(
    int Id,
    int AccountId,
    string MaskedCardNumber,
    string ExpirationMonthYear,
    bool IsActive,
    DateTime CreatedAt)
{
    public static AccountCardResponse FromEntity(AccountCard card)
    {
        return new AccountCardResponse(
            card.Id,
            card.AccountId,
            CardMasking.Mask(card.CardNumber),
            card.ExpirationMonthYear,
            card.IsActive,
            card.CreatedAt);
    }
}

public sealed record AccountResponse(
    int Id,
    int CustomerId,
    string AccountName,
    string AccountNumber,
    string Iban,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyCollection<AccountCardResponse> AccountCards)
{
    public static AccountResponse FromEntity(Account account)
    {
        return new AccountResponse(
            account.Id,
            account.CustomerId,
            account.AccountName,
            account.AccountNumber,
            account.Iban,
            account.IsActive,
            account.CreatedAt,
            account.AccountCards.Select(AccountCardResponse.FromEntity).ToArray());
    }
}

public sealed record CreditCardResponse(
    int Id,
    int CustomerId,
    string MaskedCardNumber,
    string ExpirationMonthYear,
    decimal CardLimit,
    bool IsActive,
    DateTime CreatedAt)
{
    public static CreditCardResponse FromEntity(CreditCard card)
    {
        return new CreditCardResponse(
            card.Id,
            card.CustomerId,
            CardMasking.Mask(card.CardNumber),
            card.ExpirationMonthYear,
            card.CardLimit,
            card.IsActive,
            card.CreatedAt);
    }
}

public sealed record CustomerResponse(
    int Id,
    string FullName,
    string BirthPlace,
    DateTime BirthDate,
    decimal RiskLimit,
    DateTime CreatedAt,
    IReadOnlyCollection<AccountResponse> Accounts,
    IReadOnlyCollection<CreditCardResponse> CreditCards)
{
    public static CustomerResponse FromEntity(Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.FullName,
            customer.BirthPlace,
            customer.BirthDate,
            customer.RiskLimit,
            customer.CreatedAt,
            customer.Accounts.Select(AccountResponse.FromEntity).ToArray(),
            customer.CreditCards.Select(CreditCardResponse.FromEntity).ToArray());
    }
}

public sealed record TransactionResponse(
    int Id,
    int? AccountId,
    int? CreditCardId,
    int TransactionTypeId,
    string? TransactionType,
    decimal Amount,
    string? Description,
    DateTime CreatedAt)
{
    public static TransactionResponse FromEntity(Transaction transaction)
    {
        return new TransactionResponse(
            transaction.Id,
            transaction.AccountId,
            transaction.CreditCardId,
            transaction.TransactionTypeId,
            transaction.TransactionType?.TypeName,
            transaction.Amount,
            transaction.Description,
            transaction.CreatedAt);
    }
}

internal static class CardMasking
{
    public static string Mask(string cardNumber)
    {
        return cardNumber.Length < 4
            ? "****"
            : new string('*', cardNumber.Length - 4) + cardNumber[^4..];
    }
}
