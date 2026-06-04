using food_service.ProductService.API.GlobalExceptions;

namespace food_service.ProductService.API.Middlewares
{
    public class CustomGlobalException
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomGlobalException> _logger;

        public CustomGlobalException(RequestDelegate next, ILogger<CustomGlobalException> logger)
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
                ProductNotFoundException => StatusCodes.Status404NotFound,
                NotFoundCategoryException => StatusCodes.Status404NotFound,
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
