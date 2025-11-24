namespace Havit.Blazor.Components.Web.Bootstrap;
public class HxDynamicModalService : IHxDynamicModalService
{
	private HxDynamicModal _dynamicModal;
	public Task<T> ShowAsync<T>(Type componentType, string title, Dictionary<string, object> parameters = null, ModalSize size = ModalSize.Large)
	{
		if (_dynamicModal == null)
		{
			throw new InvalidOperationException("HxDynamicModalService is not initialized. Make sure you have an HxDynamicModal component in your application.");
		}

		return _dynamicModal.ShowAsync<T>(componentType, title, parameters, size);
	}

	public void Close(object result)
	{
		_dynamicModal?.Close(result);
	}

	internal void RegisterDynamicModal(HxDynamicModal dynamicModal)
	{
		_dynamicModal = dynamicModal;
	}
}
