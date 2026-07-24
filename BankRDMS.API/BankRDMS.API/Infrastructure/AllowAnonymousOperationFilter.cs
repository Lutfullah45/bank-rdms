using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BankRDMS.API.Infrastructure;

public sealed class AllowAnonymousOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionAllowsAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();
        var controllerAllowsAnonymous = context.MethodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any() == true;

        if (actionAllowsAnonymous || controllerAllowsAnonymous)
        {
            operation.Security = [];
        }
    }
}
