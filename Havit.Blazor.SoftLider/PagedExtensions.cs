using Havit.Blazor.Components.Web.Bootstrap;
using SoftLider.Common;

namespace Havit.Blazor.SoftLider;

public static class PagedExtensions
{
	public static PagedRequest ToPagedRequest<T>(this GridDataProviderRequest<T> request) => new()
	{
		StartIndex = request.StartIndex,
		Count = request.Count ?? 10,
		Sorting = request.Sorting.Count > 0 ? $"{request.Sorting[0].SortString} {request.Sorting[0].SortDirection}" : string.Empty
	};

	public static GridDataProviderResult<T> ToGridDataProviderResult<T>(this PagedResponse<T> response) where T : class => new()
	{
		Data = response.Data,
		TotalCount = response.TotalCount
	};

	public static PagedRequest ToPagedRequest(this AutosuggestDataProviderRequest request) => new()
	{
		StartIndex = 0,
		Count = 10,
		Sorting = string.Empty,
		Filter = request.UserInput
	};

	public static AutosuggestDataProviderResult<T> ToAutosuggestDataProviderResult<T>(this PagedResponse<T> response) where T : class => new()
	{
		Data = response.Data,
	};
}
