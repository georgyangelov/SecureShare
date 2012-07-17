function updateGrid() {
	$('.thumbnails').trigger('resize');
}

function ChannelView() {
	var self = this;

	/* Properties */
	this.Name = ko.observable("Test channel");
	this.Description = ko.observable("Hey there!");

	/* Methods */
	this.updateGrid = function () {
		updateGrid();
	};
}

$('a[data-toggle="tab"]').live('shown', function (e) {
	updateGrid();
});