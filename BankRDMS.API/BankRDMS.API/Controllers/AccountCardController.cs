using System.ComponentModel.DataAnnotations;
using BankRDMS.API.Models.Responses;
using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountCardController : ControllerBase
{
    private readonly IAccountCardService _accountCardService;

    public AccountCardController(IAccountCardService accountCardService)
    {
        _accountCardService = accountCardService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccountCard(
        [FromQuery, Range(1, int.MaxValue)] int accountId)
    {
        await _accountCardService.CreateAccountCardAsync(accountId);
        return Ok("Banka kartı başarıyla oluşturuldu ve hesaba bağlandı.");
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> ToggleCardStatus(int id, [FromBody] bool isActive)
    {
        var isSuccess = await _accountCardService.UpdateStatusAsync(id, isActive);
        return isSuccess
            ? Ok(new { Message = "Kart durumu başarıyla güncellendi." })
            : NotFound(new { Message = "Kart bulunamadı." });
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAllAccountCards()
    {
        var result = (await _accountCardService.GetAllAccountCardsAsync())
            .Select(AccountCardResponse.FromEntity);
        return Ok(result);
    }

    [HttpGet("getbyid/{id:int}")]
    public IActionResult GetById(int id)
    {
        var card = _accountCardService.GetAccountCardById(id);
        return card is null
            ? NotFound("Banka kartı bulunamadı.")
            : Ok(AccountCardResponse.FromEntity(card));
    }

    [HttpGet("getbyaccountid/{accountId:int}")]
    public IActionResult GetByAccountId(int accountId)
    {
        var cards = _accountCardService.GetAccountCardsByAccountId(accountId);
        return cards.Count == 0
            ? NotFound("Hesaba ait banka kartı bulunamadı.")
            : Ok(cards.Select(AccountCardResponse.FromEntity));
    }
}
