
namespace Havit.Blazor.Components.Web.Bootstrap;

public interface IHxDynamicModalService
{
	void Close(object result);
	Task<T> ShowAsync<T>(Type componentType, string title, Dictionary<string, object> parameters = null, ModalSize size = ModalSize.Large);
}