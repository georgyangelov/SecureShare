ko.bindingHandlers.optionalText = {
	update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
		var value = ko.utils.unwrapObservable(valueAccessor());

		if (value != null && value != "") {
			$(element).show();
			$(element).text(value);
		}
		else {
			$(element).hide();
		}
	}
};