using user_service.userservice.api.CustomExceptionService;

namespace user_service.userservice.api.GlobalExceptionMiddleware
{
    public class GlobalExceptions
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptions> _logger;

        public GlobalExceptions(RequestDelegate next, ILogger<GlobalExceptions> logger)
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
                ValidationNotAccept => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
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
