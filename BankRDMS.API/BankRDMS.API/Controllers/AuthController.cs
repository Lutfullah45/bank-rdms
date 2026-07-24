using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankRDMS.API.Models.Requests;
using BankRDMS.Core.Entities;
using BankRDMS.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace BankRDMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var customer = await _authService.AuthenticateAsync(
            request.IdentityNumber,
            request.BirthDate);

        if (customer is null)
        {
            return Unauthorized(new { Message = "Kimlik numarası veya doğum tarihi hatalı." });
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(30);
        var token = GenerateToken(customer, expiresAt);

        return Ok(new
        {
            Token = token,
            ExpiresAt = expiresAt,
            CustomerId = customer.Id,
            FullName = customer.FullName
        });
    }

    private string GenerateToken(Customer customer, DateTime expiresAt)
    {
        var key = _configuration["Jwt:Key"]
                  ?? throw new InvalidOperationException("JWT anahtarı yapılandırılmamış.");
        var issuer = _configuration["Jwt:Issuer"]
                     ?? throw new InvalidOperationException("JWT issuer yapılandırılmamış.");
        var audience = _configuration["Jwt:Audience"]
                       ?? throw new InvalidOperationException("JWT audience yapılandırılmamış.");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim("customer_id", customer.Id.ToString()),
            new Claim(ClaimTypes.Name, customer.FullName),
            new Claim(ClaimTypes.Role, "Customer"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
