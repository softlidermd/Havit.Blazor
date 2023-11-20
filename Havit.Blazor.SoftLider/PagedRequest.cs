namespace Havit.Blazor.SoftLider;

public class PagedRequest
{
	public int? StartIndex { get; set; }
	public int? Count { get; set; }
	public string? Sorting { get; set; }
	public string? Filter { get; set; }

	public override string ToString()
	{
		var items = new List<string>();
		if (StartIndex is not null)
			items.Add($"startIndex={StartIndex}");
		if (Count is not null)
			items.Add($"count={Count}");
		if (Filter is not null)
			items.Add($"filter={Filter}");
		if (Sorting is not null)
			items.Add($"sorting={Sorting}");

		return string.Join("&", [.. items]);
	}
}
