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
		if (this.charAt(i).match(/[\s\.,\(\)\-\_\&\!\?\;]/) != null)
			return this.substr(0, i) + dots;

	return this.substr(0, length) + dots;
}

function toReadableFileSize(bytes) {
	var i = 0;
	var byteUnits = [' bytes' , ' kB', ' MB', ' GB', ' TB', ' PB', ' EB', ' ZB', ' YB'];

	while (bytes > 1024) {
		bytes = bytes / 1024;
		i++;
	}

	if (i > 0)
		return bytes.toFixed(1) + byteUnits[i];
	else
		return bytes + byteUnits[i];
}