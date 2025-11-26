using Evaluation.Domain.Exception;
using System.Net;
using System.Text.Json;

namespace Evaluation.API.Middleware
{
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
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred.";

            switch (ex)
            {
                case DomainException domainEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = domainEx.Message;
                    break;
                case ArgumentNullException argNullEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = argNullEx.Message;
                    break;
                case ArgumentOutOfRangeException argOutOfRangeEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = argOutOfRangeEx.Message;
                    break;
                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = argEx.Message;
                    break;
                case InvalidOperationException invalidOpEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidOpEx.Message;
                    break;
                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthorizedEx.Message;
                    break;
                case KeyNotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;
                case NotImplementedException notImplEx:
                    statusCode = HttpStatusCode.NotImplemented;
                    message = notImplEx.Message;
                    break;
                case TimeoutException timeoutEx:
                    statusCode = HttpStatusCode.RequestTimeout;
                    message = "The request timed out. Please try again later.";
                    break;
                default:
                    // Log unhandled exceptions, no expongas detalles en producción
                    break;
            }

            var response = new
            {
                success = false,
                error = message
            };

            string json = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(json);
        }
    }
}
