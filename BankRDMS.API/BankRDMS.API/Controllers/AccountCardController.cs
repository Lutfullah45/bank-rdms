using BankApi.Services;
using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
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
        public async Task<IActionResult> CreateAccountCard([FromQuery] int accountId)
        {
         
            await _accountCardService.CreateAccountCardAsync(accountId);

            return Ok("Banka kartı başarıyla oluşturuldu ve hesaba bağlandı.");
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleCardStatus(int id, [FromBody] bool isActive)
        {
           
            bool isSuccess = await _accountCardService.UpdateStatusAsync(id, isActive);

            if (isSuccess)
            {
             
                return Ok(new { Message = "Kart durumu başarıyla güncellendi." });
            }

            return BadRequest(new { Message = "Güncellenmek istenen kart bulunamadı." });
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAccountCards()
        {
            var result = await _accountCardService.GetAllAccountCardsAsync();
            return Ok(result);
        }
        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var card = _accountCardService.GetAccountCardById(id);
            if (card == null)
            {
                return NotFound("Belirtilen ID'ye ait banka kartı bulunamadı.");
            }
            return Ok(card);
        }

        [HttpGet("getbyaccountid/{accountId}")]
        public IActionResult GetByAccountId(int accountId)
        {
            var cards = _accountCardService.GetAccountCardsByAccountId(accountId);

            if (cards == null || cards.Count == 0)
            {
                return NotFound("Belirtilen hesaba ait banka kartı bulunamadı.");
            }

            return Ok(cards);
        }
    }
}