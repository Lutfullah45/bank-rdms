using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
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
        public async Task<IActionResult> CreateCreditCard([FromQuery] int customerId, [FromQuery] decimal requestedLimit)
        {
            await _creditCardService.CreateCreditCardAsync(customerId, requestedLimit);

            return Ok("Kredi kartı başarıyla oluşturuldu.");
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCreditCards()
        {
            var result = await _creditCardService.GetAllCreditCardsAsync();
            return Ok(result);
        }

        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var creditCard = _creditCardService.GetCreditCardById(id);
            if (creditCard == null)
            {
                return NotFound("Belirtilen ID'ye ait kredi kartı bulunamadı.");
            }
            return Ok(creditCard);
        }
        [HttpGet("getbycustomerid")]
        public async Task<IActionResult> GetCreditCardsByCustomerId([FromQuery] int customerId)
        {
            var result = await _creditCardService.GetCreditCardsByCustomerIdAsync(customerId);
            return Ok(result);
        }
    }
}