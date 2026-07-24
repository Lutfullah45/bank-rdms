using BankRDMS.Core.Exceptions;
using BankRDMS.Service.Abstract;

namespace BankRDMS.API.Infrastructure;

public sealed class HttpCurrentCustomer : ICurrentCustomer
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentCustomer(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int CustomerId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst("customer_id")?.Value;

            if (!int.TryParse(value, out var customerId))
            {
                throw new ForbiddenException("Müşteri kimliği token içinde bulunamadı.");
            }

            return customerId;
        }
    }
}
