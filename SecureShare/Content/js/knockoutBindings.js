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

ko.bindingHandlers.autosize = {
	init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
		var $element = $(element);
		var options = ko.utils.unwrapObservable(valueAccessor());
		
		if (typeof options.resize !== "undefined")
			$element.autosize('autosize', options.resize);

		//handle disposal
		ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
			$element.data('mirror').remove();
		});
	}
};

ko.bindingHandlers.fileUpload = {
	init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
		var $element = $(element);
		var options = ko.utils.unwrapObservable(valueAccessor());
		
		$element.fileupload(options);
	}
};

ko.bindingHandlers.masonry = {
	init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
		var $element = $(element);
		var options = ko.utils.unwrapObservable(valueAccessor());

		$element.masonry(options);
	}
};