using MeowWoofSocial.Data.DTO.Custom;

namespace MeowWoofSocial.API.Middleware;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex is CustomException customEx && customEx.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) ? StatusCodes.Status404NotFound
                : ex is CustomException
                ? StatusCodes.Status400BadRequest
                : StatusCodes.Status500InternalServerError;

            var reponse = new
            {
                StatusCodes = context.Response.StatusCode,
                ex.Message,
                Detailed = ex.ToString()
            };
            await context.Response.WriteAsJsonAsync(reponse);
        }
    }
}