using FinanceSplit.Data.Interfaces;

namespace FinanceSplit.Api.Middleware;

public class MigrationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IMigrationState migrationState)
    {
        if (!migrationState.IsDone)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new { Message = "Service Unavailable: Database migrations are currently running." });
            return;
        }

        await next(context);
    }
}
