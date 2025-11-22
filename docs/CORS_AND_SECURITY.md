# CORS and Security Headers Configuration

This document explains the CORS (Cross-Origin Resource Sharing) and security headers configuration for FlowingLinks.

## Overview

FlowingLinks uses multiple layers of CORS and security header management:

1. **ASP.NET Core CORS Policy** - Backend policy configuration
2. **Security Headers Middleware** - Backend response headers
3. **Nginx Security Configuration** - Frontend Nginx server headers

## CORS Configuration

### Backend CORS Policy (Program.cs)

The backend uses the `AllowAll` CORS policy:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

This is registered in the middleware pipeline:
```csharp
app.UseCors("AllowAll");
```

**Important**: While this allows cross-origin requests, it should be restricted in production to specific origins. See [Production Configuration](#production-configuration) below.

### Security Headers Middleware

The `SecurityHeadersMiddleware` adds essential security headers to all responses:

```csharp
X-Content-Type-Options: nosniff
X-Frame-Options: SAMEORIGIN
X-XSS-Protection: 1; mode=block
Referrer-Policy: no-referrer-when-downgrade
Permissions-Policy: geolocation=(), microphone=(), camera=()
Access-Control-Allow-Origin: <origin>
Access-Control-Allow-Credentials: true
```

### Nginx Security Configuration

Frontend Nginx also adds security headers:

```nginx
add_header X-Frame-Options "SAMEORIGIN" always;
add_header X-Content-Type-Options "nosniff" always;
add_header X-XSS-Protection "1; mode=block" always;
add_header Referrer-Policy "no-referrer-when-downgrade" always;
```

## Understanding the Headers

| Header | Purpose | Value |
|--------|---------|-------|
| `X-Content-Type-Options` | Prevents MIME-type sniffing | `nosniff` |
| `X-Frame-Options` | Prevents clickjacking | `SAMEORIGIN` |
| `X-XSS-Protection` | Browser XSS protection | `1; mode=block` |
| `Referrer-Policy` | Controls referrer information | `no-referrer-when-downgrade` |
| `Permissions-Policy` | Controls browser features | Denies geolocation, microphone, camera |
| `Access-Control-Allow-Origin` | CORS origin | The requesting origin |
| `Access-Control-Allow-Credentials` | Allow credentials in CORS | `true` |

## Referrer-Policy Explanation

The `strict-origin-when-cross-origin` error occurs when:
1. A cross-origin request is made
2. The Referrer-Policy is set too restrictively
3. The browser blocks the referrer header

**Solution**: Using `no-referrer-when-downgrade` means:
- Full referrer is sent for same-origin requests
- Full referrer is sent when going from HTTPS to HTTPS
- No referrer is sent when going from HTTPS to HTTP
- This is the most compatible setting for modern applications

## Production Configuration

For production deployments, restrict the CORS policy to specific origins:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy
            .WithOrigins(
                "https://yourdomain.com",
                "https://www.yourdomain.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

Then use it in the middleware:
```csharp
app.UseCors("AllowSpecificOrigins");
```

### Environment-Based Configuration

To make this configurable via environment variables, you can:

```csharp
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "*" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("ConfiguredOrigins", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

Then configure in appsettings.json:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  }
}
```

## Troubleshooting CORS Issues

### "CORS policy: No 'Access-Control-Allow-Origin' header"

**Cause**: Backend not returning CORS headers
**Solution**:
1. Ensure `UseCors()` is called in Program.cs
2. Check that CORS policy allows the origin
3. Verify SecurityHeadersMiddleware is enabled

### "strict-origin-when-cross-origin" Error

**Cause**: Referrer-Policy too restrictive
**Solution**:
1. Set Referrer-Policy to `no-referrer-when-downgrade`
2. Ensure both frontend and backend set this header
3. Check browser console for exact issue

### Preflight Requests Failing

**Cause**: OPTIONS requests not handled correctly
**Solution**:
1. SecurityHeadersMiddleware handles OPTIONS requests
2. CORS policy should allow preflight requests
3. Check that appropriate headers are returned

## Testing CORS

### Using curl

```bash
# Simple request
curl -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: Content-Type" \
  -X OPTIONS \
  http://localhost:5000/api/auth

# Check response headers
curl -i -H "Origin: http://localhost:3000" \
  http://localhost:5000/health
```

### Using browser DevTools

1. Open browser DevTools (F12)
2. Go to Network tab
3. Make an API request
4. Click the request and check Response Headers
5. Look for `Access-Control-Allow-Origin` header

## Related Files

- `Backend/FlowingLinks.Api/Program.cs` - CORS and security configuration
- `Backend/FlowingLinks.Api/Middleware/SecurityHeadersMiddleware.cs` - Security headers
- `Frontend/nginx.conf` - Nginx security headers
