using System.ComponentModel.DataAnnotations;

namespace BankRDMS.API.Models.Requests;

public sealed class CreateCustomerRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Kimlik numarası 11 rakamdan oluşmalıdır.")]
    public string IdentityNumber { get; init; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string BirthPlace { get; init; } = string.Empty;

    [Required]
    public DateTime BirthDate { get; init; }

}
