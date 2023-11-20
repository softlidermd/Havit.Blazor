namespace Havit.Blazor.SoftLider;

public static partial class DateTimeExtensions
{
	public static DateTime StartOfDay(this DateTime theDate)
	{
		return theDate.Date;
	}

	public static DateTime EndOfDay(this DateTime theDate)
	{
		return theDate.Date.AddDays(1).AddTicks(-1);
	}

	public static DateTime StartOfMonth(this DateTime theDate)
	{
		return new DateTime(theDate.Year, theDate.Month, 1);
	}

	public static DateTime EndOfMonth(this DateTime theDate)
	{
		return new DateTime(theDate.Year, theDate.Month, 1).AddMonths(1).AddTicks(-1);
	}

	public static DateTime StartOfYear(this DateTime theDate)
	{
		return new DateTime(theDate.Year, 1, 1);
	}

	public static DateTime EndOfYear(this DateTime theDate)
	{
		return new DateTime(theDate.Year, 1, 1).AddYears(1).AddTicks(-1);
	}
}