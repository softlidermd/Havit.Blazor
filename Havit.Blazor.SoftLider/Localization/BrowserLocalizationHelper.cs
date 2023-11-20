using Microsoft.AspNetCore.Components;

namespace Havit.Blazor.SoftLider.Localization;

public class BrowserLocalizationHelper : ILocalizationHelper
{
	private readonly NavigationManager _navigationManager;

	public BrowserLocalizationHelper(NavigationManager navigationManager)
	{
		_navigationManager = navigationManager;
	}

	public Task ChangeCultureAsync(string cultureCode)
	{
		var uri = new Uri(_navigationManager.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
		var query = $"?culture={Uri.EscapeDataString(cultureCode)}&redirectUri={Uri.EscapeDataString(uri)}";
		_navigationManager.NavigateTo("/setCulture" + query, forceLoad: true);
		return Task.CompletedTask;
	}
}
