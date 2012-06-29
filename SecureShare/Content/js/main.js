function updateGrid() {
	$('.thumbnails').trigger('resize');
}

function initGrid($) {
	$('.thumbnails').masonry({
		itemSelector: '.span',
		columnWidth: function (containerWidth) {
			return containerWidth / 6;
		}
	});

	// Fix for the script in Firefox 13. It seems to need to recalculate something
	updateGrid();
}

jQuery(initGrid);

var testText = 'Message text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about here';
$('.message.active').live('click', function () {
	var text = '';
	var $msg = $(this);
	var $thumbnail = $msg.find('.thumbnail');

	if ($msg.hasClass('span4')) {
		text = testText;

		$thumbnail
			.css('width', $thumbnail.width() + 'px')
			.css('height', $thumbnail.height() + 'px');

		$msg.removeClass('span4').addClass('span8');

		var newHeight = $thumbnail.find('p').parent().textHeight(text, $msg.width()) + $msg.find('h4').height();

		$msg.height(newHeight + 10);

		$thumbnail.animate({ width: $msg.width() - 10, height: newHeight }, 700, function () {
			updateGrid();
		});

		$thumbnail.find('p').text(text);
		updateGrid();
	}
	else {
		var $measure = $('<li><div class="thumbnail"></div></li>').addClass('span4');
		$msg.parent().append($measure);
		var span4Width = $measure.find('.thumbnail').width();
		$measure.remove();

		text = testText.cutToWord(200, '...');

		$thumbnail
			.css('width', $thumbnail.width() + 'px')
			.css('height', $thumbnail.height() + 'px');

		var newHeight = $msg.find('p').parent().textHeight(text, span4Width) + $msg.find('h4').height();

		$thumbnail.animate({ width: span4Width, height: newHeight }, 700, function () {
			$msg.addClass('span4').removeClass('span8');
			$msg.css('height', $thumbnail.outerHeight() + 'px');
			$(this).find('p').text(text);

			updateGrid();
		});

	}
});