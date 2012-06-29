(function ($) {
	$.fn.textHeight = function (text, width) {
		var sensor = $('<p />').addClass('test').css({ display: 'none', width: width, margin: 0, padding: 0 }).text(text);
		$(this).append(sensor);
		var height = sensor.height();
		sensor.remove();
		return height;
	};
})(jQuery);

String.prototype.cutToWord = function (length, dots) {
	if (typeof dots === 'undefined')
		dots = '';

	if (this.length <= length)
		return this;

	for (var i = length; i > 0; i--)
		if (this.charAt(i).match('[\.,\(\)\-\_\&\!\?\;]') != null)
			return this.substr(0, i) + dots;

	return this.substr(0, length) + dots;
}