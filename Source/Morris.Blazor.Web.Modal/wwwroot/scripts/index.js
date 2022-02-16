var MorrisBlazorWeb = MorrisBlazorWeb || {};

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

MorrisBlazorWeb.originalValueMarker = 'morris-blazor-web_original-value-';
MorrisBlazorWeb.nullValueMarker = 'morris-blazor-web_null-value';
MorrisBlazorWeb.disableControls = function (element, preserveCurrent) {
	const preserve = function (element, attributeName, newValue) {
		const originalValue = element.getAttribute(attributeName) || MorrisBlazorWeb.nullValueMarker;
		const preservedAttributeName = MorrisBlazorWeb.originalValueMarker + attributeName;
		if (element.getAttribute(preservedAttributeName) === null)
			element.setAttribute(preservedAttributeName, originalValue);
		element.setAttribute(attributeName, newValue);
	}
	if (preserveCurrent && element.getAttribute) {
		preserve(element, "disabled", true);
		preserve(element, "tabindex", -1);
	}
	for (var i = 0; i < element.childNodes.length; i++)
		MorrisBlazorWeb.disableControls(element.childNodes[i], true);
};

MorrisBlazorWeb.disableControlsById = function (id) {
	const element = document.getElementById(id);
	if (!element)
		return false;
	return MorrisBlazorWeb.disableControls(element);
}

MorrisBlazorWeb.restoreControls = function (element, restoreCurrent) {
	const restore = function (element, attributeName, newValue) {
		const preservedAttributeName = MorrisBlazorWeb.originalValueMarker + attributeName;
		const preservedValue = element.getAttribute(preservedAttributeName) || MorrisBlazorWeb.nullValueMarker;
		if (preservedValue === MorrisBlazorWeb.nullValueMarker)
			element.removeAttribute(attributeName);
		else
			element.setAttribute(attributeName, preservedValue);
		element.removeAttribute(preservedAttributeName);
	}
	if (restoreCurrent && element.getAttribute) {
		restore(element, "disabled");
		restore(element, "tabindex");
	}
	for (var i = 0; i < element.childNodes.length; i++)
		MorrisBlazorWeb.restoreControls(element.childNodes[i], true);
}

MorrisBlazorWeb.restoreControlsById = function (id) {
	const element = document.getElementById(id);
	if (!element)
		return false;
	return MorrisBlazorWeb.restoreControls(element);
}
