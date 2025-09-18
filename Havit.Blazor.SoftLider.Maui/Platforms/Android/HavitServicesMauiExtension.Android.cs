using Microsoft.AspNetCore.Components.WebView.Maui;

namespace Havit.Blazor.SoftLider.Maui
{
	public static class HavitServicesMauiExtension
	{
		public static IServiceCollection AddModalManager(this IServiceCollection services)
		{
			if ((OperatingSystem.IsBrowser() || OperatingSystem.IsWindows()))
			{
				services.AddScoped<ModalManager>();
			}
			else
			{
				services.AddSingleton<ModalManager>();
			}

#if ANDROID
			services.ConfigureMauiHandlers(h =>
			{
				h.AddHandler<BlazorWebView, HavitBlazorWebViewHandler>();
			});
#endif
			return services;
		}
	}
}
