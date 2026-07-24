using System.ComponentModel.DataAnnotations;
using BankRDMS.API.Models.Responses;
using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(
        [FromQuery, Required, StringLength(16, MinimumLength = 16)] string cardNumber,
        [FromQuery, Range(typeof(decimal), "0.01", "100000000")] decimal amount,
        [FromQuery, Required, StringLength(100)] string description)
    {
        await _transactionService.DepositAsync(cardNumber, amount, description);
        return Ok("Para yatırma işlemi başarıyla tamamlandı.");
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(
        [FromQuery, Required, StringLength(16, MinimumLength = 16)] string cardNumber,
        [FromQuery, Range(typeof(decimal), "0.01", "100000000")] decimal amount,
        [FromQuery, Required, StringLength(100)] string description)
    {
        await _transactionService.WithdrawAsync(cardNumber, amount, description);
        return Ok("Para çekme işlemi başarıyla tamamlandı.");
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase(
        [FromQuery, Required, StringLength(16, MinimumLength = 16)] string creditCardNumber,
        [FromQuery, Range(typeof(decimal), "0.01", "100000000")] decimal amount,
        [FromQuery, Required, StringLength(100)] string description)
    {
        await _transactionService.PurchaseAsync(creditCardNumber, amount, description);
        return Ok("Harcama işlemi başarıyla tamamlandı.");
    }

    [HttpPost("transfer-to-credit-card")]
    public async Task<IActionResult> TransferToCreditCard(
        [FromQuery, Required, StringLength(16, MinimumLength = 16)] string accountCardNumber,
        [FromQuery, Required, StringLength(16, MinimumLength = 16)] string creditCardNumber,
        [FromQuery, Range(typeof(decimal), "0.01", "100000000")] decimal amount,
        [FromQuery, Required, StringLength(100)] string description)
    {
        await _transactionService.TransferToCreditCardAsync(
            accountCardNumber,
            creditCardNumber,
            amount,
            description);
        return Ok("Kredi kartı ödeme işlemi başarıyla tamamlandı.");
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAllTransactions()
    {
        var result = (await _transactionService.GetAllTransactionsAsync())
            .Select(TransactionResponse.FromEntity);
        return Ok(result);
    }

    [HttpGet("getbyid/{id:int}")]
    public IActionResult GetById(int id)
    {
        var transaction = _transactionService.GetTransactionById(id);
        return transaction is null
            ? NotFound("İşlem bulunamadı.")
            : Ok(TransactionResponse.FromEntity(transaction));
    }
}
