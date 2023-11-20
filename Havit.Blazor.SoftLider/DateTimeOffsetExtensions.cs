namespace Havit.Blazor.SoftLider;

public static class DateTimeOffsetExtensions
{
	public static DateTimeOffset Truncate(this DateTimeOffset dateTime, TimeSpan timeSpan)
	{
		if (timeSpan == TimeSpan.Zero) return dateTime; // Or could throw an ArgumentException
		if (dateTime == DateTimeOffset.MinValue || dateTime == DateTimeOffset.MaxValue) return dateTime; // do not modify "guard" values
		return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
	}

	public static DateTimeOffset StartOfDay(this DateTimeOffset theDate)
	{
		return new DateTimeOffset(theDate.Year, theDate.Month, theDate.Day, 0, 0, 0, theDate.Offset);
	}

	public static DateTimeOffset EndOfDay(this DateTimeOffset theDate)
	{
		return new DateTimeOffset(theDate.Year, theDate.Month, theDate.Day, 0, 0, 0, theDate.Offset).AddDays(1).AddTicks(-1);
	}

	public static DateTimeOffset StartOfMonth(this DateTimeOffset theDate)
	{
		return new DateTimeOffset(theDate.Year, theDate.Month, 1, 0, 0, 0, theDate.Offset);
	}

	public static DateTimeOffset EndOfMonth(this DateTimeOffset theDate)
	{
		return new DateTimeOffset(theDate.Year, theDate.Month, 1, 0, 0, 0, theDate.Offset).AddMonths(1).AddTicks(-1);
	}

	public static DateTimeOffset StartOfYear(this DateTimeOffset theDate)
	{
		return new DateTimeOffset(theDate.Year, 1, 1, 0, 0, 0, theDate.Offset);
	}

	public static DateTimeOffset EndOfYear(this DateTimeOffset theDate)
	{
		return new DateTimeOffset(theDate.Year, 1, 1, 0, 0, 0, theDate.Offset).AddYears(1).AddTicks(-1);
	}
}
