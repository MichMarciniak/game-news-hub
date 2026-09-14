namespace GameNewsHub.Api.Features.Shared;

public static class DateCalculator
{
    public static DateTimeOffset GetMonthStart(DateTimeOffset date)
    {
        var utc = date.ToUniversalTime();
        return new DateTimeOffset(utc.Year, utc.Month, 1, 0, 0, 0, TimeSpan.Zero);
    }

    public static DateTimeOffset GetMonthEnd(DateTimeOffset date)
    {
        return GetMonthStart(date).AddMonths(1).AddTicks(-1);
    }

    public static (DateTimeOffset From, DateTimeOffset To) GetDefaultRange(DateTimeOffset now)
    {
        return (GetMonthStart(now), GetMonthEnd(now));
    }
    
    public static (DateTimeOffset From, DateTimeOffset To) GetDateRange(DateTime? from, DateTime? to)
    {
        var now = DateTimeOffset.UtcNow;
        var (defaultFrom, defaultTo) = DateCalculator.GetDefaultRange(now);
        var rangeFrom = from?.ToUniversalTime() ?? defaultFrom;
        var rangeTo = to?.ToUniversalTime() ?? defaultTo;
        return (rangeFrom, rangeTo);
    }

    public static (DateTimeOffset From, DateTimeOffset To) GetDateRange(DateTimeOffset? from, DateTimeOffset? to)
    {
        var now = DateTimeOffset.UtcNow;
        var (defaultFrom, defaultTo) = DateCalculator.GetDefaultRange(now);
        var rangeFrom = from?.ToUniversalTime() ?? defaultFrom;
        var rangeTo = to?.ToUniversalTime() ?? defaultTo;
        return (rangeFrom, rangeTo);
    }
}