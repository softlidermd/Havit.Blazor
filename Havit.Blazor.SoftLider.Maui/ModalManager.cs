using Microsoft.JSInterop;

namespace Havit.Blazor.SoftLider.Maui
{
	public class ModalManager : IAsyncDisposable, IDisposable
	{
		private ModalManager() { }

		private static ModalManager? _instance;

		public static ModalManager GetInstance()
		{
			if (_instance == null)
			{
				_instance = new ModalManager();
			}

			return _instance;
		}

		private string jsCode =
			"""
			(function () {
				let dotnet = null;
				let shownHandler = null;
				let hiddenHandler = null;
				let idCounter = 0;

				function ensureId(el) {
					if (!el.id) {
						el.id = "modal-" + (++idCounter);
					}
					return el.id;
				}

				window.modalInterop = {
					init: function (dotnetRef) {
						dotnet = dotnetRef;

						// Bootstrap 5 uses native events that bubble. We can listen on document.
						shownHandler = function (e) {
							const el = e.target;
							const id = ensureId(el);
							dotnet.invokeMethodAsync("ModalOpened", id);
						};
						hiddenHandler = function (e) {
							const el = e.target;
							const id = ensureId(el);
							dotnet.invokeMethodAsync("ModalClosed", id);
						};

						keydownHandler = function (e) {
							if (e.key === "Escape") {
								dotnet.invokeMethodAsync("OnEscapeKeyPressed");
							}
						};

						document.addEventListener("shown.bs.modal", shownHandler, true);
						document.addEventListener("hidden.bs.modal", hiddenHandler, true);
					},

					dispose: function () {
						if (shownHandler) document.removeEventListener("shown.bs.modal", shownHandler, true);
						if (hiddenHandler) document.removeEventListener("hidden.bs.modal", hiddenHandler, true);
						shownHandler = hiddenHandler = null;
						dotnet = null;
					},

					hideById: function (id) {
						const el = document.getElementById(id);
						if (!el)
							return;
						// Requires Bootstrap 5's JS to be loaded (window.bootstrap)
						const modal = window.bootstrap?.Modal.getOrCreateInstance(el);
						modal?.hide();
					}
				};
			})();
			""";
		private DotNetObjectReference<ModalManager>? _ref;
		private readonly object _gate = new();
		private readonly Stack<string> _stack = new();
		private IJSRuntime? _js;

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

		public async Task InitAsync(IJSRuntime js)
		{
			_js = js;
			_ref = DotNetObjectReference.Create<ModalManager>(this);
			await _js.InvokeVoidAsync("eval", jsCode);
			await _js.InvokeVoidAsync("modalInterop.init", _ref);
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
				// Avoid duplicates (defensive)
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

				// Remove the given id wherever it is in the stack.
				var arr = _stack.ToArray(); // top -> bottom
				_stack.Clear();
				for (int i = arr.Length - 1; i >= 0; i--) // rebuild bottom -> top
				{
					var current = arr[i];
					if (!string.Equals(current, id, StringComparison.Ordinal))
					{
						_stack.Push(current);
					}
				}
			}
		}

		// Called from native back handler (synchronous). Schedules JS call to hide top modal.
		public async void CloseTop()
		{
			string? id = null;
			lock (_gate)
			{
				if (_stack.Count > 0)
				{
					id = _stack.Peek();
				}
			}

			if (id is null)
			{
				return;
			}

			var js = _js;

			if (js is null)
			{
				return;
			}

			// Make the JS call on the UI thread without blocking the back handler.
			try
			{
				await js.InvokeVoidAsync("modalInterop.hideById", id);
			}
			catch
			{
				// ignore
			}
		}

		public void Dispose()
		{
			_ = _js?.InvokeVoidAsync("modalInterop.dispose");
			_ref?.Dispose();
		}

		public async ValueTask DisposeAsync()
		{
			if (_js != null)
			{
				await _js.InvokeVoidAsync("modalInterop.dispose");
			}

			_ref?.Dispose();
		}
	}
}
