using ErrorOr;
using GameNewsHub.Api.Features.Events;
using GameNewsHub.Contracts.Events;
using GameNewsHub.Contracts.Shared;
using GameNewsHub.Data;
using GameNewsHub.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameNewsHub.Api.Features.Admin;

public class AdminService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        AppDbContext context,
        ILogger<AdminService> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> ReassignPlatformGroup(int platformId, int groupId)
    {
        var platform = await _context.Platforms.FindAsync(platformId);

        if (platform == null)
        {
            _logger.LogWarning(
                "Reassign failed: Platform {PlatformId} not found",
                platformId);
            return Error.NotFound("Platform.NotFound",
                $"Platform {platformId} not found.");
        }

        var newGroup = await _context.PlatformGroups.FindAsync(groupId);

        if (newGroup == null)
        {
            _logger.LogWarning(
                "Reassign failed: PlatformGroup {PlatformGroupId} not found",
                groupId);
            return Error.NotFound("PlatformGroup.NotFound",
                $"Platform group {groupId} not found.");
        }

        var oldGroupId = platform.PlatformGroupId;
        platform.PlatformGroup = newGroup;
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Platform {PlatformId} reassigned from group {OldPlatformGroupId} to {NewPlatformGroupId}",
            platformId, oldGroupId, groupId);

        return new ErrorOr<Success>();
    }
    public async Task<ErrorOr<Success>> ReassignEventToSeries(int eventId, int seriesId)
    {
        var evt = await _context.Events
            .Include(e => e.Series)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (evt == null)
        {
            _logger.LogWarning(
                "Reassign failed: Event {EventId} not found",
                eventId);
            return Error.NotFound("Event.NotFound", $"Event {eventId} not found");
        }

        var series = await _context.EventSeries
            .FirstOrDefaultAsync(s => s.Id == seriesId);

        if (series == null)
        {
            _logger.LogWarning(
                "Reassign failed: EventSeries {SeriesId} not found",
                seriesId);
            return Error.NotFound("EventSeries.NotFound", $"EventSeries {seriesId} not found");
        }

        var oldSeriesId = evt.EventSeriesId;
        evt.Series = series;
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Event {EventId} reassigned from series {OldSeriesId} to {NewSeriesId}",
            evt.Id, oldSeriesId, seriesId);

        return new ErrorOr<Success>();
    }
    public async Task<SearchDto<AdminSeriesDto>> GetAdminSeries(string? query, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        IQueryable<EventSeries> dbQuery = _context.EventSeries
            .Where(s => s.Events.Any());

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim();
            dbQuery = dbQuery
                .Where(s => s.Events
                    .Any(e => EF.Functions.ILike(e.Name, $"%{query}%")));
        };

        var total = await dbQuery.CountAsync();

        var series = await dbQuery
            .OrderByDescending(s => s.Events.Max(e => e.StartTime))
            .ThenBy(s => s.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(s => s.Events)
            .ToListAsync();

        var items = series
            .Select(s =>
            {
                var result = SeriesMapper.ToAdminDto(s);
                result.DisplayName = s.Events
                    .OrderByDescending(e => e.StartTime)
                    .Select(e => e.NormalizedName)
                    .FirstOrDefault() ?? string.Empty;
                result.Events = s.Events
                    .Select(EventMapper.ToListItemDto)
                    .ToList();
                return result;
            })
            .ToList();

        return new SearchDto<AdminSeriesDto>
        {
            Total = total,
            Items = items
        };
    }
}