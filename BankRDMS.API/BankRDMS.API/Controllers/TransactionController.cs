using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
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
        public async Task<IActionResult> Deposit([FromQuery] string cardNumber, [FromQuery] decimal amount, [FromQuery] string description)
        {
            try
            {
                await _transactionService.DepositAsync(cardNumber, amount, description);
                return Ok("Deposit transaction completed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromQuery] string cardNumber, [FromQuery] decimal amount, [FromQuery] string description)
        {
            try
            {
                await _transactionService.WithdrawAsync(cardNumber, amount, description);
                return Ok("Withdraw transaction completed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase([FromQuery] string creditCardNumber, [FromQuery] decimal amount, [FromQuery] string description)
        {
            try
            {
                await _transactionService.PurchaseAsync(creditCardNumber, amount, description);
                return Ok("Purchase transaction completed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer-to-credit-card")]
        public async Task<IActionResult> TransferToCreditCard([FromQuery] string accountCardNumber, [FromQuery] string creditCardNumber, [FromQuery] decimal amount, [FromQuery] string description)
        {
            try
            {
                await _transactionService.TransferToCreditCardAsync(accountCardNumber, creditCardNumber, amount, description);
                return Ok("Transfer to credit card completed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var result = await _transactionService.GetAllTransactionsAsync();
            return Ok(result);
        }

        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var transaction = _transactionService.GetTransactionById(id);
            if (transaction == null)
            {
                return NotFound("Transaction not found for the given ID.");
            }
            return Ok(transaction);
        }
    }
}