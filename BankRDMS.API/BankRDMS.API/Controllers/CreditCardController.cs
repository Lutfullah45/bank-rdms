using System.ComponentModel.DataAnnotations;
using BankRDMS.API.Models.Responses;
using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CreditCardController : ControllerBase
{
    private readonly ICreditCardService _creditCardService;

    public CreditCardController(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCreditCard(
        [FromQuery, Range(1, int.MaxValue)] int customerId,
        [FromQuery, Range(typeof(decimal), "1", "100000000")] decimal requestedLimit)
    {
        await _creditCardService.CreateCreditCardAsync(customerId, requestedLimit);
        return Ok("Kredi kartı başarıyla oluşturuldu.");
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAllCreditCards()
    {
        var result = (await _creditCardService.GetAllCreditCardsAsync())
            .Select(CreditCardResponse.FromEntity);
        return Ok(result);
    }

    [HttpGet("getbyid/{id:int}")]
    public IActionResult GetById(int id)
    {
        var creditCard = _creditCardService.GetCreditCardById(id);
        return creditCard is null
            ? NotFound("Kredi kartı bulunamadı.")
            : Ok(CreditCardResponse.FromEntity(creditCard));
    }

    [HttpGet("getbycustomerid")]
    public async Task<IActionResult> GetCreditCardsByCustomerId(
        [FromQuery, Range(1, int.MaxValue)] int customerId)
    {
        var result = (await _creditCardService.GetCreditCardsByCustomerIdAsync(customerId))
            .Select(CreditCardResponse.FromEntity);
        return Ok(result);
    }
}
