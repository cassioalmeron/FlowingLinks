namespace FlowingLinks.Api.Middleware
{
    /// <summary>
    /// Middleware to add security headers to HTTP responses
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
            // Add security headers
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "no-referrer-when-downgrade");
            context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            // CORS headers - allow requests from same origin and configured origins
            if (context.Request.Headers.ContainsKey("Origin"))
            {
                var origin = context.Request.Headers["Origin"].ToString();
                context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            }

            // Handle preflight requests
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                context.Response.StatusCode = 200;
                return;
            }

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
