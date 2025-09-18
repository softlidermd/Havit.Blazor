using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Havit.Blazor.SoftLider.Maui
{
	public static class HavitServicesExtension
	{
		public static IServiceCollection AddHavitBlazorWebViewHandler(this IServiceCollection services)
		{
			services.ConfigureMauiHandlers(h =>
			{
				h.AddHandler<BlazorWebView, HavitBlazorWebViewHandler>();
			});

			return services;
		}
	}
}
