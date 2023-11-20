using System.Runtime.Versioning;
using System.Text.Json;

namespace Havit.Blazor.SoftLider.PersistentStorage;

public interface IPersistentStorage
{
	[UnsupportedOSPlatform("browser")]
	string? GetItem(string key);
	[UnsupportedOSPlatform("browser")]
	void SetItem(string key, string value);

	Task<string?> GetItemAsync(string key);
	Task SetItemAsync(string key, string value);

	async Task<T?> GetItemAsync<T>(string key)
	{
		var serializedValue = await GetItemAsync(key);
		if (serializedValue == null)
			return default;
		return JsonSerializer.Deserialize<T>(serializedValue);
	}

	async Task SetItemAsync<T>(string key, T value)
	{
		var serializedValue = JsonSerializer.Serialize(value);
		await SetItemAsync(key, serializedValue);
	}
}
