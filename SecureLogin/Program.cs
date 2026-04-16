using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SecureLogin;
using SecureLogin.Model;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("Logs/api-log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Level:u3}] {Timestamp:yyyy-MM-dd HH:mm:ss} | Host: {MachineName} | Client: {ClientName} | IP: {ClientIp} | Method: {ApiMethod} | Params: {RequestParams} | Msg: {Message}{NewLine}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(); // Rješava transient failure grešku
    }));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Enter your API Key (GUID format)",
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         var context = services.GetRequiredService<UserDbContext>();
//         context.Database.Migrate();
//         if (!context.Clients.Any())
//         {
//             context.Clients.Add(new Client
//             {
//                 ClientName = "Admin Test Client",
//                 ApiKey = Guid.Parse("F91BDAB9-14FC-40FF-B9E8-7ACE33EE4EE2")
//             });
//
//             context.SaveChanges();
//             Log.Information("The database was empty. The initial client was successfully inserted.");
//         }
//     }
//     catch (Exception ex)
//     {
//         var logger = services.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "An error occurred while migrating the database.");
//     }
// }

app.UseSwagger();
app.UseSwaggerUI(c => c.DefaultModelsExpandDepth(-1));


app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    if (path.StartsWith("/swagger") || path.StartsWith("/v3/api-docs"))
    {
        await next();
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { error = "API Key is missing." });
        return;
    }

    if (!Guid.TryParse(extractedKey, out Guid apiKeyGuid))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key format." });
        return;
    }


    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var client = await db.Clients
            .Where(c => c.ApiKey == apiKeyGuid)
            .Select(c => new { c.ClientName })
            .FirstOrDefaultAsync();

        if (client == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized access." });
            return;
        }

        // Serilog Context logovanje
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        using (LogContext.PushProperty("ClientName", client.ClientName))
        using (LogContext.PushProperty("ClientIp", clientIp))
        using (LogContext.PushProperty("ApiMethod", $"{context.Request.Method} {context.Request.Path}"))
        {
            Log.Information("API authorized.");
            await next();
        }
    }
});

app.MapControllers();
app.Run();

public partial class Program { }
