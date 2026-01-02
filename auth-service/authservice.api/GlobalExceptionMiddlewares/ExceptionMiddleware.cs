using auth_service.authservice.api.CustomExceptionSerives;

namespace auth_service.authservice.api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, ex.Message);
                await HandleException(context, ex);
            }
        }

        private static Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                RevokeTokenFailException => StatusCodes.Status508LoopDetected,

                _ => StatusCodes.Status500InternalServerError
            };

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = ex.Message
            };

            return context.Response.WriteAsJsonAsync(response);
        }

    }
}
