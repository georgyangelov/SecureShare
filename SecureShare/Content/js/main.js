function initGrid($)
{
	$('.thumbnails').masonry({
		itemSelector: '.span',
		columnWidth: function (containerWidth) {
			return containerWidth / 6;
		}
	});

	// Fix for the script in Firefox 13. It seems to need to recalculate something
	$('.thumbnails').trigger('resize');
}

jQuery(initGrid);