using TrimUrlApi.Exceptions;

namespace TrimUrlApi.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/json";
                if (ex is ApiException apiEx)
                {
                    context.Response.StatusCode = apiEx.StatusCode;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.Message
                });
            }
        }
    }
}
