using Microsoft.JSInterop;

namespace Havit.Blazor.SoftLider.Maui
{
	public partial class ModalManager : IAsyncDisposable, IDisposable
	{
		private ModalManager() { }

		private static ModalManager _instance;

		public static ModalManager GetInstance()
		{
			if (_instance == null)
				_instance = new ModalManager();

			return _instance;
		}

		private string jsCode = "(function () {\r\n\tlet dotnet = null;\r\n\tlet shownHandler = null;\r\n\tlet hiddenHandler = null;\r\n\tlet idCounter = 0;\r\n\r\n\tfunction ensureId(el) {\r\n\t\tif (!el.id) {\r\n\t\t\tel.id = \"modal-\" + (++idCounter);\r\n\t\t}\r\n\t\treturn el.id;\r\n\t}\r\n\r\n\twindow.modalInterop = {\r\n\t\tinit: function (dotnetRef) {\r\n\t\t\tdotnet = dotnetRef;\r\n\r\n\t\t\t// Bootstrap 5 uses native events that bubble. We can listen on document.\r\n\t\t\tshownHandler = function (e) {\r\n\t\t\t\tconst el = e.target;\r\n\t\t\t\tconst id = ensureId(el);\r\n\t\t\t\tdotnet.invokeMethodAsync(\"ModalOpened\", id);\r\n\t\t\t};\r\n\t\t\thiddenHandler = function (e) {\r\n\t\t\t\tconst el = e.target;\r\n\t\t\t\tconst id = ensureId(el);\r\n\t\t\t\tdotnet.invokeMethodAsync(\"ModalClosed\", id);\r\n\t\t\t};\r\n\r\n\t\t\tdocument.addEventListener(\"shown.bs.modal\", shownHandler, true);\r\n\t\t\tdocument.addEventListener(\"hidden.bs.modal\", hiddenHandler, true);\r\n\t\t},\r\n\r\n\t\tdispose: function () {\r\n\t\t\tif (shownHandler) document.removeEventListener(\"shown.bs.modal\", shownHandler, true);\r\n\t\t\tif (hiddenHandler) document.removeEventListener(\"hidden.bs.modal\", hiddenHandler, true);\r\n\t\t\tshownHandler = hiddenHandler = null;\r\n\t\t\tdotnet = null;\r\n\t\t},\r\n\r\n\t\thideById: function (id) {\r\n\t\t\tconst el = document.getElementById(id);\r\n\t\t\tif (!el)\r\n\t\t\t\treturn;\r\n\t\t\t// Requires Bootstrap 5's JS to be loaded (window.bootstrap)\r\n\t\t\tconst modal = window.bootstrap?.Modal.getOrCreateInstance(el);\r\n\t\t\tmodal?.hide();\r\n\t\t}\r\n\t};\r\n})();";
		private DotNetObjectReference<ModalManager>? _ref;
		private readonly object _gate = new();
		private readonly Stack<string> _stack = new();
		private IJSRuntime? _js;

		public bool HasOpenModal
		{
			get { lock (_gate) return _stack.Count > 0; }
		}

		public async Task Init(IJSRuntime js)
		{
			_js = js;
			_ref = DotNetObjectReference.Create<ModalManager>(this);
			await _js.InvokeVoidAsync("eval", jsCode);
			await _js.InvokeVoidAsync("modalInterop.init", _ref);
		}

		[JSInvokable]
		public void ModalOpened(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return;
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
			if (string.IsNullOrWhiteSpace(id)) return;
			lock (_gate)
			{
				if (_stack.Count == 0) return;

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
			await _js.InvokeVoidAsync("modalInterop.dispose");
			_ref?.Dispose();
		}
	}
}
