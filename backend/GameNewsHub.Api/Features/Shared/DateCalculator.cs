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
}