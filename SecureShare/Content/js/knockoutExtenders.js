ko.extenders.validation = function (target, options) {
	target.isValid = ko.observable(true);
	target.isModified = ko.observable(false);

	target.hasError = ko.computed(function () {
		return !target.isValid();
	});
	target.hasModError = ko.computed(function () {
		return target.isModified() && !target.isValid();
	});

	target.errorMessage = ko.observable("");

	if (typeof options.func === "function") {
		var computedFunc = ko.computed(options.func);
	}

	function validate(v, modify) {
		var value = target();
		var messages = [];
		var validity = true;

		if (typeof modify === "undefined" || modify)
			target.isModified(true);

		if ((options.required && value == "")
				||
		    (typeof options.regex !== "undefined" && !options.regex.test(value))
			    ||
            (typeof options.func === "function" && !options.func(value))
		   ) {
			validity = false;
			target.errorMessage(options.message);
		}

		target.isValid(validity);
	}

	validate(target());
	target.isModified(false);

	target.subscribe(validate);

	if (typeof computedFunc === "function")
		computedFunc.subscribe(function (v) {
			validate(v, false);
		});

	return target;
};