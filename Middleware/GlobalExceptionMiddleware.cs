using System.Net;
using System.Text.Json;
using ubuntu_health_api.Exceptions;

namespace ubuntu_health_api.Middleware
{
  public class GlobalExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
      catch (Exception ex)
      {
        _logger.LogError(ex, "An unhandled exception occurred");
        await HandleExceptionAsync(context, ex);
      }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      context.Response.ContentType = "application/json";

      var response = new ErrorResponse();

      switch (exception)
      {
        case ValidationException validationEx:
          response.StatusCode = (int)HttpStatusCode.BadRequest;
          response.Message = "Validation failed";
          response.Details = validationEx.Errors;
          break;

        case NotFoundException notFoundEx:
          response.StatusCode = (int)HttpStatusCode.NotFound;
          response.Message = notFoundEx.Message;
          break;

        case ConflictException conflictEx:
          response.StatusCode = (int)HttpStatusCode.Conflict;
          response.Message = conflictEx.Message;
          break;

        case UnauthorizedAccessException unauthorizedEx:
          response.StatusCode = (int)HttpStatusCode.Forbidden;
          response.Message = unauthorizedEx.Message;
          break;

        default:
          response.StatusCode = (int)HttpStatusCode.InternalServerError;
          response.Message = "An internal server error occurred";
          break;
      }

      context.Response.StatusCode = response.StatusCode;

      var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });

      await context.Response.WriteAsync(jsonResponse);
    }
  }

  public class ErrorResponse
  {
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
  }
}
