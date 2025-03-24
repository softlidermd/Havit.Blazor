using Havit.Blazor.Components.Web.Bootstrap;
using SoftLider.Common;

namespace Havit.Blazor.SoftLider;

public static class DateTimeRanges
{
	public static InputDateRangePredefinedRangesItem[] GetPredefinedRanges()
	{
		var predefinedDateRanges = new[]
		{
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.ThisWeek,
				DateRange = new() { StartDate = DateTime.Today.StartOfWeek(), EndDate = DateTime.Today.EndOfWeek() }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.PreviousWeek,
				DateRange = new() { StartDate = DateTime.Today.StartOfWeek().AddDays(-7), EndDate = DateTime.Today.StartOfWeek().AddTicks(-1) }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.Last7Days,
				DateRange = new() { StartDate = DateTime.Today.AddDays(-7), EndDate = DateTime.Today.EndOfDay() }
			},
			new InputDateRangePredefinedRangesItem()
			{
				Label = Resources.Localization.Previous7Days,
				DateRange = new() { StartDate = DateTime.Today.AddDays(-14), EndDate = DateTime.Today.AddDays(-7).AddTicks(-1) }
			},
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
