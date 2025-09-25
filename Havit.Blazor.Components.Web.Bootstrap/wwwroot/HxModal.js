// HxModal.js
// - http/https: JS-only back-to-close for Bootstrap modals (stack-aware).
// - file://: no history logic; use native Back. Provides window.hxModalApi for native calls.
//
// Requires Bootstrap 5. Invokes .NET callbacks:
//   HxModal_HandleModalShown, HxModal_HandleModalHide, HxModal_HandleModalHidden

const isFile = (location.protocol === 'file:');

// http/https-only state
let hxModalHistoryStack = []; // modal element IDs we pushed into history (topmost last)
let hxBackListenerWired = false;
let hxIgnoreNextPop = 0;      // ignore our own programmatic forward/back
let hxPopInProgress = false;  // true while handling a Back attempt
let hxBackTargetId = null;    // modal id being closed due to Back

function ensureId(el) {
	if (!el.id) {
		el.id = 'hx-modal-' + Math.random().toString(36).slice(2, 9);
	}
	return el.id;
}

// Keep Blazor NavigationManager's state shape consistent
function getCurrentIndex(state) {
	const s = state != null ? state : history.state;
	return s && typeof s._index === 'number' ? s._index : 0;
}
function cloneUserState(state) {
	const s = state != null ? state : history.state;
	return s && Object.prototype.hasOwnProperty.call(s, 'userState') ? s.userState : undefined;
}
function pushModalState(id, baseState) {
	const baseIdx = getCurrentIndex(baseState);
	const newIdx = baseIdx + 1;
	const newState = { userState: cloneUserState(baseState), _index: newIdx, hxModal: true, id };
	history.pushState(newState, ''); // no URL change
}

function ensureBackListener() {
	if (hxBackListenerWired || isFile) return; // web only

	// Intercept Back before Blazor/router
	window.addEventListener('popstate', (ev) => {
		// Ignore our own programmatic forward/back
		if (hxIgnoreNextPop > 0) {
			hxIgnoreNextPop--;
			return;
		}

		if (hxModalHistoryStack.length === 0) return;

		// Stop Blazor/router from reacting
		try { ev.stopImmediatePropagation(); ev.stopPropagation(); } catch { }

		// Close the topmost modal we own
		while (hxModalHistoryStack.length > 0) {
			const topId = hxModalHistoryStack[hxModalHistoryStack.length - 1];
			const el = document.getElementById(topId);
			if (!el) { hxModalHistoryStack.pop(); continue; }

			// Neutralize this Back now; we'll honor it after the modal actually closes
			try { hxIgnoreNextPop++; history.forward(); } catch { }

			try {
				const inst = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
				hxPopInProgress = true;
				hxBackTargetId = topId;
				inst.hide(); // hide prevention handled in handleModalHide
			} catch { }
			break;
		}
	}, true); // capture phase

	hxBackListenerWired = true;
}

// Public API (module exports)

export function show(element, hxModalDotnetObjectReference, closeOnEscape, subscribeToHideEvent) {
	if (!element || bootstrap.Modal.getInstance(element)) return;

	element.hxModalDotnetObjectReference = hxModalDotnetObjectReference;
	if (subscribeToHideEvent) {
		element.addEventListener('hide.bs.modal', handleModalHide);
	}
	element.addEventListener('hidden.bs.modal', handleModalHidden);
	element.addEventListener('shown.bs.modal', handleModalShown);

	const modalCount = document.querySelectorAll('.modal').length; // already includes current modal
	const modal = new bootstrap.Modal(element, { keyboard: closeOnEscape });

	if (modalCount > 1) {
		const modalNumber = Math.min(modalCount, 5);
		try {
			modal._element.classList.add(`hx-modal-${modalNumber}`);
			if (modal._backdrop && modal._backdrop._config) {
				modal._backdrop._config.className = `modal-backdrop hx-modal-backdrop-${modalNumber}`;
			}
		} catch { }
	}

	modal.show();
}

export function hide(element) {
	if (!element) return;
	element.hxModalHiding = true;
	const modal = bootstrap.Modal.getInstance(element);
	if (modal) modal.hide();
}

// Internal handlers

function handleModalShown(event) {
	// .NET callback
	event.target.hxModalDotnetObjectReference.invokeMethodAsync('HxModal_HandleModalShown');

	try {
		if (!isFile) {
			ensureBackListener();

			const id = ensureId(event.target);

			// Avoid duplicate push if re-fired
			if (hxModalHistoryStack[hxModalHistoryStack.length - 1] !== id) {
				let pushed = false;
				try {
					pushModalState(id, history.state);
					pushed = true;
				} catch { }
				if (pushed) {
					hxModalHistoryStack.push(id);
					event.target.hxModalPushed = true;
				} else {
					event.target.hxModalPushed = false;
				}
			}
		} else {
			// file:// (MAUI): no history manipulation
			event.target.hxModalPushed = false;
		}
	} catch { }
}

async function handleModalHide(event) {
	const modalInstance = bootstrap.Modal.getInstance(event.target);

	if (modalInstance.hidePreventionDisabled || event.target.hxModalDisposing) {
		modalInstance.hidePreventionDisabled = false;
		return;
	}

	// Ask .NET if we can hide (honors hide prevention indefinitely)
	event.preventDefault();
	const cancel = await event.target.hxModalDotnetObjectReference.invokeMethodAsync('HxModal_HandleModalHide');

	if (cancel) {
		// If Back triggered this and it's canceled, keep modal open and clear flags
		if (hxPopInProgress && hxBackTargetId === (event.target.id || event.target.getAttribute('id'))) {
			hxPopInProgress = false;
			hxBackTargetId = null;
		}
		return;
	}

	// Proceed to hide
	modalInstance.hidePreventionDisabled = true;
	event.target.hxModalHiding = true;
	modalInstance.hide();
}

function handleModalHidden(event) {
	event.target.hxModalHiding = false;

	try {
		const id = event.target.id || event.target.getAttribute('id');
		const pushedForThisModal = !!event.target.hxModalPushed;
		event.target.hxModalPushed = false;

		// Remove from stack (topmost expected)
		const topId = hxModalHistoryStack[hxModalHistoryStack.length - 1];
		if (topId === id) {
			hxModalHistoryStack.pop();
		} else {
			const idx = hxModalHistoryStack.lastIndexOf(id);
			if (idx >= 0) hxModalHistoryStack.splice(idx, 1);
		}

		if (event.target.hxModalDisposing) {
			// Being disposed due to teardown; don't fight navigation
		} else if (!isFile && hxPopInProgress && hxBackTargetId === id) {
			// http/https: completing a Back attempt — honor the user's Back now
			hxPopInProgress = false;
			hxBackTargetId = null;
			try { hxIgnoreNextPop++; history.back(); } catch { }
		} else if (!isFile && pushedForThisModal) {
			// http/https: UI/programmatic close — remove the synthetic entry
			try { hxIgnoreNextPop++; history.back(); } catch { }
		}
	} catch { }

	if (event.target.hxModalDisposing) {
		// fix for #110 where the dispose() gets called while the offcanvas is still in hiding-transition
		dispose(event.target);
		return;
	}

	// .NET callback
	event.target.hxModalDotnetObjectReference.invokeMethodAsync('HxModal_HandleModalHidden');
}

export function dispose(element) {
	if (!element) return;

	element.hxModalDisposing = true;

	if (element.hxModalHiding) {
		// fix for #110 where the dispose() gets called while the offcanvas is still in hiding-transition
		return;
	}

	// Remove any stack reference to this element (avoid stale entries)
	try {
		const id = element.id || element.getAttribute('id');
		if (id) {
			for (let i = hxModalHistoryStack.length - 1; i >= 0; i--) {
				if (hxModalHistoryStack[i] === id) {
					hxModalHistoryStack.splice(i, 1);
					break;
				}
			}
		}
		element.hxModalPushed = false;

		if (hxBackTargetId === id) {
			hxBackTargetId = null;
			hxPopInProgress = false;
		}
	} catch { }

	element.removeEventListener('hide.bs.modal', handleModalHide);
	element.removeEventListener('hidden.bs.modal', handleModalHidden);
	element.removeEventListener('shown.bs.modal', handleModalShown);
	element.hxModalDotnetObjectReference = null;

	const modal = bootstrap.Modal.getInstance(element);
	if (modal) {
		modal.dispose();
	}
}