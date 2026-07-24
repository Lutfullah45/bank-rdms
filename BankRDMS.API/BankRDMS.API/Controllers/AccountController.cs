using System.ComponentModel.DataAnnotations;
using BankApi.Services;
using BankRDMS.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount(
        [FromQuery, Range(1, int.MaxValue)] int customerId,
        [FromQuery, StringLength(50, MinimumLength = 2)] string accountName)
    {
        await _accountService.CreateAccountAsync(customerId, accountName);
        return Ok("Hesap başarıyla oluşturuldu.");
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAllAccounts()
    {
        var result = (await _accountService.GetAllAccountsAsync())
            .Select(AccountResponse.FromEntity);
        return Ok(result);
    }

    [HttpGet("getbyid/{id:int}")]
    public IActionResult GetById(int id)
    {
        var account = _accountService.GetAccountById(id);
        return account is null
            ? NotFound("Hesap bulunamadı.")
            : Ok(AccountResponse.FromEntity(account));
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ToggleAccountStatus(int id, [FromBody] bool isActive)
    {
        var isSuccess = await _accountService.UpdateAccountStatusAsync(id, isActive);
        if (!isSuccess)
        {
            return NotFound(new { Message = "Hesap bulunamadı." });
        }

        return Ok(new
        {
            Message = $"Hesap başarıyla {(isActive ? "Aktif" : "Pasif")} duruma getirildi.",
            AccountId = id
        });
    }

    [HttpGet("getbycustomerid")]
    public async Task<IActionResult> GetAccountsByCustomerId(
        [FromQuery, Range(1, int.MaxValue)] int customerId)
    {
        var result = (await _accountService.GetAccountsByCustomerIdAsync(customerId))
            .Select(AccountResponse.FromEntity);
        return Ok(result);
    }
}
