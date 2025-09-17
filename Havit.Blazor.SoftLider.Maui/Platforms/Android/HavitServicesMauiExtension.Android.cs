using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Havit.Blazor.SoftLider.Maui
{
	public static class HavitServicesMauiExtension
	{
		public static IMauiHandlersCollection AddHavitBackButtonBehavior(this IMauiHandlersCollection services)
		{
			services.AddHandler<BlazorWebView, HavitBlazorWebViewHandler>();
			return services;
		}
	}
}
