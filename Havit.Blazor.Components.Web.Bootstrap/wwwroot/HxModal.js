export function show(element, hxModalDotnetObjectReference, closeOnEscape, subscribeToHideEvent) {
	if (!element) {
		return;
	}

	element.hxModalDotnetObjectReference = hxModalDotnetObjectReference;
	if (subscribeToHideEvent)
		element.addEventListener('hide.bs.modal', handleModalHide);
	element.addEventListener('hidden.bs.modal', handleModalHidden);
	element.addEventListener('shown.bs.modal', handleModalShown);

	var modalCount = document.querySelectorAll('.modal').length; // already includes current modal

	var modal = new bootstrap.Modal(element, {
		keyboard: closeOnEscape
	});

	if (modalCount > 1) {
		var modalNumber = Math.min(modalCount, 5);
		modal._element.classList.add(`hx-modal-${modalNumber}`);
		modal._backdrop._config.className = `modal-backdrop hx-modal-backdrop-${modalNumber}`;
	}

	if (modal) {
		modal.show();
	}
}

export function hide(element) {
	var modal = bootstrap.Modal.getInstance(element);
	if (modal) {
		modal.hide();
	}
}

function handleModalShown(event) {
	event.target.hxModalDotnetObjectReference.invokeMethodAsync('HxModal_HandleModalShown');
};

async function handleModalHide(event) {
    let modalInstance = bootstrap.Modal.getInstance(event.target);

	if (modalInstance.hidePreventionDisabled)
        return;

    event.preventDefault();

    let cancel = await event.target.hxModalDotnetObjectReference.invokeMethodAsync('HxModal_HandleModalHide');
    if (!cancel) {
		modalInstance.hidePreventionDisabled = true;
		modalInstance.hide();
		event.target.hxModalHiding = true;
    }
};

function handleModalHidden(event) {
	event.target.hxModalHiding = false;

	event.target.hxModalDotnetObjectReference.invokeMethodAsync('HxModal_HandleModalHidden');

	if (event.target.hxModalDisposing) {
		// fix for #110 where the dispose() gets called while the offcanvas is still in hiding-transition
		dispose(event.target);
		return;
	}
};

export function dispose(element) {
	if (!element) {
		return;
	}

	element.hxModalDisposing = true;

	if (element.hxModalHiding) {
		// fix for #110 where the dispose() gets called while the offcanvas is still in hiding-transition
		return;
	}

	element.removeEventListener('hide.bs.modal', handleModalHide);
	element.removeEventListener('hidden.bs.modal', handleModalHidden);
	element.removeEventListener('shown.bs.modal', handleModalShown);
	element.hxModalDotnetObjectReference = null;

	var modal = bootstrap.Modal.getInstance(element);
	if (modal) {
		modal.dispose();
	}
}