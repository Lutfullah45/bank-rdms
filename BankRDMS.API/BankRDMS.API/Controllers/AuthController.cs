using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BankRDMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BankRDMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context; 

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(string identityNumber, DateTime birthDate)
        {
            
            var customer = _context.Customers.FirstOrDefault(c =>
                c.IdentityNumber == identityNumber &&
                c.BirthDate.Date == birthDate.Date);

            if (customer == null)
            {
                return Unauthorized(new { Message = "Hatalı TC Kimlik No veya Doğum Tarihi." });
            }

           
            var token = GenerateToken(customer.IdentityNumber, customer.Id.ToString(), customer.FullName);

            return Ok(new
            {
                Token = token,
                Message = "Giriş Başarılı!",
                CustomerId = customer.Id,
                FullName = customer.FullName
            });
        }

        private string GenerateToken(string identityNumber, string customerId, string fullName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Token içerisine gömülecek kimlik bilgileri (Claims)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customerId),             // Kullanıcı ID'si
                new Claim(ClaimTypes.Name, fullName),                           // Müşteri Adı Soyadı
                new Claim("IdentityNumber", identityNumber),                    // TC Kimlik No
                new Claim(ClaimTypes.Role, "Customer")                          // Rol tanımlaması
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Token 2 saat geçerli
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}