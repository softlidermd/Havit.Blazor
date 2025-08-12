let touchStartX = 0;
let touchEndX = 0;
let touchStartY = 0;
let touchEndY = 0;

const swipeArea = document;

let leftEdgeThreshold = 30;
let minSwipeDistance = 60;

const handleTouchStart = (e) => {
	touchStartX = e.changedTouches[0].clientX;
	touchStartY = e.changedTouches[0].clientY;
	if (touchStartX > leftEdgeThreshold) touchStartX = 0; // Ignore if not near left edge
};

const handleTouchMove = (e) => {
	touchEndX = e.changedTouches[0].clientX;
	touchEndY = e.changedTouches[0].clientY;
};

const handleTouchEnd = () => {
	if (touchStartX === 0) return;

	const deltaX = touchEndX - touchStartX;
	const deltaY = touchEndY - touchStartY;

	if (Math.abs(deltaX) > Math.abs(deltaY) && deltaX > minSwipeDistance) {
		const sidebar = document.querySelector('.hx-sidebar-collapse');
		const bsCollapse = bootstrap.Collapse.getOrCreateInstance(sidebar, {
			toggle: false
		});
		bsCollapse.show();
	}

	touchStartX = 0;
	touchEndX = 0;
	touchStartY = 0;
	touchEndY = 0;
};

export function initializeSwipeDetection() {
	swipeArea.addEventListener('touchstart', handleTouchStart);
	swipeArea.addEventListener('touchmove', handleTouchMove);
	swipeArea.addEventListener('touchend', handleTouchEnd);
}

export function unregisterSwipeDetection() {
	swipeArea.removeEventListener('touchstart', handleTouchStart);
	swipeArea.removeEventListener('touchmove', handleTouchMove);
	swipeArea.removeEventListener('touchend', handleTouchEnd);
}