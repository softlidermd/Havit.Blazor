using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace Havit.Blazor.SoftLider
{
	public class ModalManager : IAsyncDisposable
	{
		public ModalManager() { }

		private DotNetObjectReference<ModalManager>? _ref;
		private readonly object _gate = new();
		private readonly Stack<string> _stack = new();
		private IJSRuntime? _js;
		private NavigationManager? _navigationManager;
		private IDisposable? _locationChangingHandler;

		public bool HasOpenModal
		{
			get
			{
				lock (_gate)
				{
					return _stack.Count > 0;
				}
			}
		}

		public async Task InitAsync(IJSRuntime js, NavigationManager navigationManager = null)
		{
			if (_js != null)
			{
				return;
			}

			if ((OperatingSystem.IsBrowser() || OperatingSystem.IsWindows()) && navigationManager != null)
			{
				_navigationManager = navigationManager;
				_locationChangingHandler = _navigationManager.RegisterLocationChangingHandler(OnLocationChangingAsync);
			}

			_js = js;
			_ref = DotNetObjectReference.Create<ModalManager>(this);
			await _js.InvokeVoidAsync("import", "/_content/Havit.Blazor.SoftLider/modalManager.js");
			await _js.InvokeVoidAsync("modalInterop.init", _ref);
		}

		private async ValueTask OnLocationChangingAsync(LocationChangingContext context)
		{
			if (this.HasOpenModal)
			{
				context.PreventNavigation();
				await this.CloseTopAsync();
			}

			await ValueTask.CompletedTask;
		}

		[JSInvokable]
		public void ModalOpened(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return;
			}

			lock (_gate)
			{
				if (!_stack.Contains(id))
				{
					_stack.Push(id);
				}
			}
		}

		[JSInvokable]
		public void ModalClosed(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return;
			}

			lock (_gate)
			{
				if (_stack.Count == 0)
				{
					return;
				}

				var arr = _stack.ToArray();
				_stack.Clear();
				for (int i = arr.Length - 1; i >= 0; i--)
				{
					var current = arr[i];
					if (!string.Equals(current, id, StringComparison.Ordinal))
					{
						_stack.Push(current);
					}
				}
			}
		}

		public async Task CloseTopAsync()
		{
			try
			{
				string id = string.Empty;
				lock (_gate)
				{
					if (_stack.Count > 0)
					{
						id = _stack.Peek();
					}
				}

				if (string.IsNullOrEmpty(id) || _js is null)
				{
					return;
				}

				await _js.InvokeVoidAsync("modalInterop.hideById", id);
			}
			catch
			{

			}
		}

		public async ValueTask DisposeAsync()
		{
			try
			{
				if (_js != null)
				{
					await _js.InvokeVoidAsync("modalInterop.dispose");
				}
			}
			finally
			{
				_locationChangingHandler?.Dispose();
				_ref?.Dispose();
			}
		}
	}
}
