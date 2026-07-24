using BankRDMS.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.API.Infrastructure;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            DomainValidationException => (
                StatusCodes.Status400BadRequest,
                "Geçersiz istek",
                exception.Message),
            EntityNotFoundException => (
                StatusCodes.Status404NotFound,
                "Kayıt bulunamadı",
                exception.Message),
            ForbiddenException => (
                StatusCodes.Status403Forbidden,
                "Bu işlem için yetkiniz yok",
                exception.Message),
            ConflictException => (
                StatusCodes.Status409Conflict,
                "İşlem mevcut verilerle çakışıyor",
                exception.Message),
            DbUpdateException => (
                StatusCodes.Status409Conflict,
                "Veri bütünlüğü ihlali",
                "İşlem benzersiz veya ilişkisel bir veritabanı kısıtıyla çakıştı."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Beklenmeyen sunucu hatası",
                "İşlem tamamlanamadı.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "İşlenmeyen hata. TraceId: {TraceId}", httpContext.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "İstek reddedildi. StatusCode: {StatusCode}, TraceId: {TraceId}",
                statusCode,
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path,
                Extensions = { ["traceId"] = httpContext.TraceIdentifier }
            },
            cancellationToken);

        return true;
    }
}
