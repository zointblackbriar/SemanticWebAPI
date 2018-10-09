using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SemanticAPI.MVC
{
    public class MVCMiddleware
    {
        private readonly RequestDelegate _next;

        public MVCMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if request body is empty and it's a multipart request
            if (context.Request.ContentLength == 0
                && context.Request.ContentType != null
                && context.Request.ContentType.ToUpper().StartsWith("MULTIPART/"))
            {
                // Set 400 response with a message
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Multipart request body must not be empty.");
            }
            else
            {
                // All other requests continue the way down the pipeline
                await _next(context);
            }
        }
    }
}
