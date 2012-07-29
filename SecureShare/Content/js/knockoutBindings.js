(function ($) {

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

	ko.bindingHandlers.onResize = {
		init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
			var throttle;
			$(element).bind('resize', function () {
				clearTimeout(throttle);
				throttle = setTimeout(valueAccessor(), 300);
			});
		}
	};

	ko.bindingHandlers.fileDropZone = {
		init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
			var $element = $(element);
			var options = ko.utils.unwrapObservable(valueAccessor());

			$(document).bind('dragover', function (e) {
				var dropZone = $element,
					timeout = window.dropZoneTimeout;
				if (!timeout) {
					dropZone.addClass('in');
				} else {
					clearTimeout(timeout);
				}
				if (e.target === dropZone[0]) {
					dropZone.addClass('hover');
				} else {
					dropZone.removeClass('hover');
				}
				window.dropZoneTimeout = setTimeout(function () {
					window.dropZoneTimeout = null;
					dropZone.removeClass('in hover');
				}, 100);
			});

			$(document).bind('drop dragover', function (e) {
				if (typeof options.drop !== "undefined")
					options.drop();

				e.preventDefault();
			});
		}
	};

	ko.bindingHandlers.imagePreview = {
		update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
			var $element = $(element);
			var options = ko.utils.unwrapObservable(valueAccessor());
			$element.children().remove();

			if (options.file() != null && /^image\/(gif|jpeg|png)$/.test(options.file().type)) {
				window.loadImage(
					options.file(),
					function (img) {
						$element.append(img);

						if (typeof options.loaded !== "undefined") {
							options.loaded($element);
						}
					},
					{
						maxWidth: 350,
						maxHeight: 350
					}
				);
			}
		}
	};

	ko.bindingHandlers.isHovering = {
		update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
			var $element = $(element);

			$element.hover(function () {
				valueAccessor()(true);
			},
			function () {
				valueAccessor()(false);
			});
		}
	};

})(jQuery);