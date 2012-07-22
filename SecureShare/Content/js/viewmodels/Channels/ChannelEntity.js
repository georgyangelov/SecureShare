function updateGrid() {
	$('.thumbnails').masonry("reload");
}

function ChannelEntity(data) {
	var self = this;

	/* Data reset */
	if (typeof data.Title === "undefined")
		data.Title = "";
	if (typeof data.Message === "undefined")
		data.Message = "";
	if (typeof data.Link === "undefined")
		data.Link = "";

	/* Properties */
	this.Id = ko.observable(data.Id);
	this.Title = ko.observable(data.Title);
	this.Message = ko.observable(data.Message);
	this.Link = ko.observable(data.Link);

	this.Importance = ko.observable(data.Importance);

	/* Helper properties */
	this.isLink = ko.computed(function () {
		return self.Link() != "";
	});

	this.smallMessage = ko.computed(function () {
		return self.Message().cutToWord(250, ' ...');
	});
	this.currentMessage = ko.observable(this.smallMessage());

	this.isExpandable = ko.computed(function () {
		return self.smallMessage() != self.Message();
	});

	var expanded = ko.observable(false);
	this.isExpanded = ko.computed({
		read: function () {
			return expanded();
		},
		write: function (value) {
			if (!self.isExpandable())
				return;

			var $msg = $("[data-id='" + self.Id() + "']");

			if (value) {
				var $thumbnail = $msg.find('.thumbnail');

				if ($msg.hasClass('span4')) {
					$thumbnail
						.css('width', $thumbnail.width() + 'px')
						.css('height', $thumbnail.height() + 'px');

					$msg.removeClass('span4').addClass('span8');

					var newHeight = $thumbnail.find('p').parent().textHeight(self.Message(), $msg.width()) + (self.Title().length ? $msg.find('h4').height() : 0) + 5;

					$msg.height(newHeight + 10);

					setTimeout(function () {
						$msg.addClass('expanded');
						self.currentMessage(self.Message());
						$thumbnail.find('.icon-resize-full').removeClass('icon-resize-full').addClass('icon-resize-small');
						$thumbnail.animate({ width: $msg.width() - 10, height: newHeight }, 700, function () {
							updateGrid();

							expanded(true);
						});
					}, 700);

					updateGrid();
				}

			}
			else {
				var $thumbnail = $msg.find('.thumbnail');

				var $measure = $('<li><div class="thumbnail"></div></li>').addClass('span4');
				$msg.parent().append($measure);
				var span4Width = $measure.find('.thumbnail').width();
				$measure.remove();

				$thumbnail
						.css('width', $thumbnail.width() + 'px')
						.css('height', $thumbnail.height() + 'px');

				var newHeight = $msg.find('p').parent().textHeight(self.smallMessage(), span4Width) + (self.Title().length ? $msg.find('h4').height() : 0) + 5;

				$thumbnail.find('.icon-resize-small').removeClass('icon-resize-small').addClass('icon-resize-full');
				$thumbnail.animate({ width: span4Width, height: newHeight }, 700, function () {
					$msg.addClass('span4').removeClass('span8');
					self.currentMessage(self.smallMessage());
					$msg.css('height', $thumbnail.outerHeight() + 'px');

					updateGrid();

					expanded(false);
				});
				$msg.removeClass('expanded');
			}
		}
	});
	this.toggleExpanded = function () {
		self.isExpanded(!self.isExpanded());

		return false;
	};
}