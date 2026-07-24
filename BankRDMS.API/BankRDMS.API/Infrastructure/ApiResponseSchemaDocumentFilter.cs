using BankRDMS.API.Models.Responses;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BankRDMS.API.Infrastructure;

public sealed class ApiResponseSchemaDocumentFilter : IDocumentFilter
{
    private static readonly Type[] ResponseTypes =
    [
        typeof(MessageResponse),
        typeof(AccountStatusResponse),
        typeof(CustomerCreatedResponse),
        typeof(LoginResponse),
        typeof(AccountCardResponse),
        typeof(AccountResponse),
        typeof(CreditCardResponse),
        typeof(CustomerResponse),
        typeof(TransactionResponse)
    ];

    public void Apply(
        Microsoft.OpenApi.OpenApiDocument swaggerDoc,
        DocumentFilterContext context)
    {
        foreach (var responseType in ResponseTypes)
        {
            context.SchemaGenerator.GenerateSchema(
                responseType,
                context.SchemaRepository);
        }
    }
}
