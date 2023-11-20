namespace Havit.Blazor.SoftLider;

public class PagedResponse<T> where T : class
{
	public IList<T> Data { get; init; } = default!;

	public int TotalCount { get; init; }
}