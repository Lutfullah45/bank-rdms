using System.ComponentModel.DataAnnotations;

namespace BankRDMS.API.Models.Requests;

public sealed class LoginRequest
{
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Kimlik numarası 11 rakamdan oluşmalıdır.")]
    public string IdentityNumber { get; init; } = string.Empty;

    [Required]
    public DateTime BirthDate { get; init; }
}
