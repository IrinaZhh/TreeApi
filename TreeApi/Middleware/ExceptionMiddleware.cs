using TreeApi.Data;
using TreeApi.Entities;
using TreeApi.Exceptions;

namespace TreeApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var eventId = DateTime.UtcNow.Ticks;

            db.Journals.Add(new ExceptionJournal
            {
                EventId = eventId,
                CreatedAt = DateTime.UtcNow,
                Text = ex.ToString()
            });

            await db.SaveChangesAsync();

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            if (ex is SecureException)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    type = ex.GetType().Name.Replace("Exception", ""),
                    id = eventId,
                    data = new { message = ex.Message }
                });
            }
            else
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "Exception",
                    id = eventId,
                    data = new { message = $"Internal server error ID = {eventId}" }
                });
            }
        }
    }
}