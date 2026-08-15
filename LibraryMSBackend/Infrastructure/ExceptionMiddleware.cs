using FluentValidation;
using System.Net;
using System.Text.Json;

namespace LibraryMSBackend.Infrastructure
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = ex.Errors.Select(e => e.ErrorMessage);
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors }));
            }
            catch (InvalidOperationException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors = new[] { ex.Message } }));
            }
        }
    }
}