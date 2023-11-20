using Havit.Blazor.Components.Web.Bootstrap;

namespace Havit.Blazor.SoftLider;

public static class DateTimeRanges
{
	public static InputDateRangePredefinedRangesItem[] GetPredefinedRanges()
	{
		var predefinedDateRanges = new[]
		{
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.ThisMonth,
				DateRange = new() { StartDate = DateTime.Today.StartOfMonth(), EndDate = DateTime.Today.EndOfMonth() }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.PreviousMonth,
				DateRange = new() { StartDate = DateTime.Today.AddMonths(-1).StartOfMonth(), EndDate = DateTime.Today.AddMonths(-1).EndOfMonth() }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.Last30Days,
				DateRange = new() { StartDate = DateTime.Today.AddDays(-30), EndDate = DateTime.Today }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.ThisYear,
				DateRange = new() { StartDate = DateTime.Today.StartOfYear(), EndDate = DateTime.Today.EndOfYear() }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.PreviousYear,
				DateRange = new() { StartDate = DateTime.Today.AddYears(-1).StartOfYear(), EndDate = DateTime.Today.AddYears(-1).EndOfYear() }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.Last365Days,
				DateRange = new() { StartDate = DateTime.Today.AddDays(-365), EndDate = DateTime.Today }
			},
		};
		return predefinedDateRanges;
	}
}
