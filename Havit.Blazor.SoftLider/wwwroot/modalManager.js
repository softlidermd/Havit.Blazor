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