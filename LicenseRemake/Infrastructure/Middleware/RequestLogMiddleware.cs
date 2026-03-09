using System.Text;
using LicenseRemake.Domain;
using LicenseRemake.Infrastructure;

namespace LicenseRemake.Infrastructure.Middleware;
// знаю что такое практикуют и приветствуют в реализации. но четкого понятния что тут происходит нет. делал вместе с чатом
public class RequestLogMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DataDbContext dbContext)
    {
        var request = context.Request;

        request.EnableBuffering();

        string body = string.Empty;

        if (request.ContentLength > 0 &&
            request.ContentType != null &&
            request.ContentType.Contains("application/json"))
        {
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                leaveOpen: true);

            body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        await _next(context);

        var log = new RequestLog
        {
            Endpoint = request.Path,
            Method = request.Method,
            Body = body,
            Ip = ip,
            StatusCode = context.Response.StatusCode,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.RequestLogs.Add(log);
        await dbContext.SaveChangesAsync();
    }
}
