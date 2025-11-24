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

            switch(ex)
            {
                case DomainException domainEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = domainEx.Message;
                    break;
                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthorizedEx.Message;
                    break;
                case KeyNotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;
                default:
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
