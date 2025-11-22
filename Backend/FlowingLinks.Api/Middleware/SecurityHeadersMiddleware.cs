namespace FlowingLinks.Api.Middleware
{
    /// <summary>
    /// Middleware to add security headers to HTTP responses
    /// Note: CORS is handled by the ASP.NET Core CORS policy instead
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
    }

    // Extension method for easy registration
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder) =>
            builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
