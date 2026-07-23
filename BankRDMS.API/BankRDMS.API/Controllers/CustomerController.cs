using BankRDMS.Core.Entities;
using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BankRDMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("create")]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

         
            if (customer.Accounts == null || customer.Accounts.Count == 0)
            {
                customer.Accounts = new List<Account>
                {
                    new Account
                    {
                        AccountName = "Vadesiz Hesap",
                        AccountNumber = GenerateAccountNumber(),
                        Iban = GenerateIban(),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow, 
                        
                        // YENİ EKLENEN KISIM: Vadesiz hesaba bağlı banka kartı oluşturuluyor
                        AccountCards = new List<AccountCard>
                        {
                            new AccountCard
                            {
                                CardNumber = GenerateCardNumber(),
                                Ccv = GenerateCvv(),
                                ExpirationMonthYear = DateTime.UtcNow.AddYears(5).ToString("MM/yy"), // Kart süresi 5 yıl
                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            }
                        }
                    }
                };
            }

            // Servis üzerinden müşteri, hesap ve kart oluşturma işlemini çağır
            var result = _customerService.AddCustomerWithAccount(customer);

            // Sonuca göre HTTP 200 (Ok) veya HTTP 400 (BadRequest) dön
            if (result)
            {
                return Ok(new { Message = "Müşteri, ilişkili vadesiz hesap ve banka kartı başarıyla oluşturuldu." });
            }

            return BadRequest(new { Message = "Müşteri oluşturulurken veritabanı aşamasında bir hata meydana geldi." });
        }

        [HttpGet("getall")]
        public IActionResult GetAllCustomers()
        {
            var customers = _customerService.GetAllCustomers();

            if (customers == null || customers.Count == 0)
            {
                return NotFound(new { Message = "Sistemde henüz kayıtlı müşteri bulunmamaktadır." });
            }

            return Ok(customers);
        }

        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound("Belirtilen ID'ye ait müşteri bulunamadı.");
            }
            return Ok(customer);
        }

        // --- YARDIMCI METOTLAR ---
        // Sınıfın içinde, ancak dışarıdan erişime kapalı (private) olacak şekilde tanımlandılar.

        private string GenerateAccountNumber()
        {
            Random rnd = new Random();
            return rnd.Next(10000000, 99999999).ToString();
        }

        private string GenerateIban()
        {
            // Basit bir IBAN simülasyonu
            return "TR" + GenerateAccountNumber() + GenerateAccountNumber() + "0000";
        }

        private string GenerateCardNumber()
        {
            Random rnd = new Random();
            // Örnek bir BIN numarası (ilk 8 hane) ile başlatıp son 8 haneyi rastgele üretiyoruz.
            string firstPart = "40001234";
            string secondPart = rnd.Next(10000000, 99999999).ToString();
            return firstPart + secondPart;
        }

        private string GenerateCvv()
        {
            Random rnd = new Random();
            return rnd.Next(100, 999).ToString();
        }
    }
}