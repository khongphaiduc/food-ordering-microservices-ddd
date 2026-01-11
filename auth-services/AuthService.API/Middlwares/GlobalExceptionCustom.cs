using auth_services.AuthService.API.CustomExceptions;

namespace auth_services.AuthService.API.Middlwares
{
    public class GlobalExceptionCustom
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionCustom> _logger;

        public GlobalExceptionCustom(RequestDelegate nex, ILogger<GlobalExceptionCustom> logger)
        {
            _next = nex;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // chuyển qua middleware tiếp theo
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
                NotfoundExceptions => StatusCodes.Status404NotFound,
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
