using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Splitwise.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try { await _next(context); }
        catch (UnauthorizedAccessException ex) { await Write(context, HttpStatusCode.Unauthorized, ex.Message); }
        catch (InvalidOperationException ex)  { await Write(context, HttpStatusCode.BadRequest, ex.Message); }
        catch (ArgumentException ex)         { await Write(context, HttpStatusCode.BadRequest, ex.Message); }
        catch (KeyNotFoundException)         { await Write(context, HttpStatusCode.NotFound, "Not found"); }
        catch (DbUpdateConcurrencyException) { await Write(context, HttpStatusCode.Conflict, "Concurrency conflict"); }
        catch (Exception)                    { await Write(context, HttpStatusCode.InternalServerError, "Unexpected error"); }
    }
    private static Task Write(HttpContext ctx, HttpStatusCode code, string message)
    {
        ctx.Response.StatusCode = (int)code;
        ctx.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { error = message });
        return ctx.Response.WriteAsync(payload);
    }
}
