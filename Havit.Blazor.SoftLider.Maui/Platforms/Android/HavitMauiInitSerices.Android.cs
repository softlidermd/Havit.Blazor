namespace Havit.Blazor.SoftLider.Maui.Platforms;

public class HavitInitializeServices
{
	public static IServiceCollection Initialize(IServiceCollection services)
	{
		services.AddSingleton<ModalManager>(x => ModalManager.GetInstance());
		return services;
	}
}
