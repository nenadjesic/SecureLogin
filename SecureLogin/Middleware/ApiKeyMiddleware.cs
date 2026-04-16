using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using SecureLogin;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string APIKEYNAME = "X-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, UserDbContext dbContext)
    {
        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            await RespondUnauthorized(context, "API Key is missing.");
            return;
        }

        
        if (!Guid.TryParse(extractedApiKey.ToString(), out Guid apiKeyGuid))
        {
            await RespondUnauthorized(context, "API Key format is invalid.");
            return;
        }

        
        var clientName = await dbContext.Clients
            .AsNoTracking()
            .Where(c => c.ApiKey == apiKeyGuid)
            .Select(c => c.ClientName)
            .FirstOrDefaultAsync();

        if (clientName == null)
        {
            await RespondUnauthorized(context, "Invalid API Key.");
            return;
        }

        
        context.Items["ClientName"] = clientName;
        await _next(context);
    }

    private static async Task RespondUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}

