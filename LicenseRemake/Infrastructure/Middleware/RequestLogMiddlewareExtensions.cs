namespace LicenseRemake.Infrastructure.Middleware;
// как и не понимаю для чего этот экстеншн
public static class RequestLogMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLogMiddleware>();
    }
}
