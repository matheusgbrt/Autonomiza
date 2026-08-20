using GestaoAutonomo.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GestaoAutonomo.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = ex.Message,
                Status = ex.StatusCode
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ao processar a requisição.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Ocorreu um erro inesperado.",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
