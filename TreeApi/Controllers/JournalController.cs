using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeApi.Data;
using TreeApi.Exceptions;
using TreeApi.Models;

namespace TreeApi.Controllers;

[ApiController]
public class JournalController : ControllerBase
{
    private readonly AppDbContext _db;

    public JournalController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("/api.user.journal.getSingle")]
    public async Task<MJournal> GetSingle([FromQuery] long id)
    {
        var item = await _db.Journals.FindAsync(id);
        if (item == null)
            throw new SecureException("Event not found");

        return new MJournal
        {
            Id = item.Id,
            EventId = item.EventId,
            CreatedAt = item.CreatedAt,
            Text = item.Text
        };
    }

    [HttpPost("/api.user.journal.getRange")]
    public async Task<MRange_MJournalInfo> GetRange(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromBody] VJournalFilter? filter)
    {
        var query = _db.Journals.AsQueryable();

        if (filter?.From != null)
            query = query.Where(x => x.CreatedAt >= filter.From);

        if (filter?.To != null)
            query = query.Where(x => x.CreatedAt <= filter.To);

        if (!string.IsNullOrWhiteSpace(filter?.Search))
            query = query.Where(x => x.Text.Contains(filter.Search));

        var count = await query.CountAsync();

        var items = await query
            .Skip(skip)
            .Take(take)
            .Select(x => new MJournalInfo
            {
                Id = x.Id,
                EventId = x.EventId,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return new MRange_MJournalInfo
        {
            Skip = skip,
            Count = count,
            Items = items
        };
    }
}