var MorrisBlazorWeb = MorrisBlazorWeb || {};

MorrisBlazorWeb.showModal = function (dialogId) {
	document.getElementById(dialogId).showModal();
}

MorrisBlazorWeb.canFocus = function (element) {
	if (!(element.tabIndex >= 0) || element.disabled)
		return false;

	switch (element.nodeName) {
		case 'A':
			return element.rel != 'ignore';

		case 'INPUT':
			return element.type != 'hidden';

		case 'BUTTON':
		case 'SELECT':
		case 'TEXTAREA':
			return true;

		default:
			return false;
	}
}

MorrisBlazorWeb.setFocus = function (element) {
	if (!MorrisBlazorWeb.canFocus(element))
		return false;

	try {
		element.focus();
	} catch {
	}
	return document.activeElement === element;
};

MorrisBlazorWeb.focusFirstAvailableControl = function (element) {
	if (MorrisBlazorWeb.setFocus(element))
		return true;

	for (var i = 0; i < element.childNodes.length; i++) {
		var child = element.childNodes[i];
		if (MorrisBlazorWeb.focusFirstAvailableControl(child))
			return true;
	}
	return false;
};

MorrisBlazorWeb.focusFirstAvailableControlById = function (id) {
	const element = document.getElementById(id);
	if (!element)
		return false;
	return MorrisBlazorWeb.focusFirstAvailableControl(element);
}
