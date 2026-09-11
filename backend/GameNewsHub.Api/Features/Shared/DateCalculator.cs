namespace GameNewsHub.Api.Features.Shared;

public static class DateCalculator
{
    public static DateTimeOffset GetMonthStart(DateTimeOffset date)
    {
        return new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, date.Offset);
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