function initGrid($)
{
	$('.thumbnails').masonry({
		itemSelector: '.span',
		columnWidth: function (containerWidth) {
			return containerWidth / 6;
		}
	});
}

jQuery(initGrid);