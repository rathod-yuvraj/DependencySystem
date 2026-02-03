using DependencySystem.Helper;

using System.Net;
using System.Text.Json;

namespace DependencySystem.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<string>.Fail(ex.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
