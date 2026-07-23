using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
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
        public async Task<IActionResult> CreateAccount([FromQuery] int customerId, [FromQuery] string accountName)
        {
          
            await _accountService.CreateAccountAsync(customerId, accountName);

            return Ok("Hesap başarıyla oluşturuldu.");
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var result = await _accountService.GetAllAccountsAsync();
            return Ok(result);
        }

        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)
            {
                return NotFound("Belirtilen ID'ye ait hesap bulunamadı.");
            }
            return Ok(account);
        }

        [HttpPut("{id}/status")]
        public IActionResult ToggleAccountStatus(int id, [FromBody] bool isActive)
        {
            
            var isSuccess = _accountService.UpdateAccountStatus(id, isActive);

            if (!isSuccess)
            {
                return NotFound(new { Message = "Belirtilen ID'ye sahip hesap bulunamadı." });
            }

            var statusMessage = isActive ? "Aktif" : "Pasif";
            return Ok(new { Message = $"Hesap başarıyla {statusMessage} duruma getirildi.", AccountId = id });
        }

        [HttpGet("getbycustomerid")]
        public async Task<IActionResult> GetAccountsByCustomerId([FromQuery] int customerId)
        {
            var result = await _accountService.GetAccountsByCustomerIdAsync(customerId);
            return Ok(result);
        }
    }
}