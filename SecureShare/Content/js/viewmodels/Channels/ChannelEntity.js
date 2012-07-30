function updateGrid() {
	$('.thumbnails').masonry("reload");
}

function ChannelEntity(data) {
	var self = this;

	/* Properties */
	this.Id = ko.observable(data.Id);
	this.Title = ko.observable(data.Title || "");
	this.Message = ko.observable(data.Message || "");
	this.Link = ko.observable(data.Link || "");

	this.IsFile = ko.observable(data.IsFile || false);
	this.FileName = ko.observable(data.FileName || "");
	this.FileLink = ko.observable(data.FileLink + "?SessionKey=" + Application.user().SessionKey.Key() || "");
	this.FileLength = ko.observable(data.FileLength || 0);
	this.FilePreview = ko.observable(data.FilePreview + "?SessionKey=" + Application.user().SessionKey.Key() || "");
	this.FilePreviewLength = ko.observable(data.FilePreviewLength || 0);

	this.Importance = ko.observable(data.Importance || 1);

	/* Helper properties */
	this.isLink = ko.computed(function () {
		return self.Link() != "";
	});

	this.smallMessage = ko.computed(function () {
		return self.Message().cutToWord(250, ' ...');
	});
	this.currentMessage = ko.observable(this.smallMessage());

	this.isExpandable = ko.computed(function () {
		return self.FilePreviewLength() > 0 || self.smallMessage() != self.Message();
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

			var smallClass = 'span4',
				bigClass   = 'span8';
			if ($msg.is('.file')) {
				smallClass = 'span2';
				bigClass   = 'span4';
			}

			if (value) {
				var $thumbnail = $msg.find('.thumbnail');

				$thumbnail
					.css('width', $thumbnail.width() + 'px')
					.css('height', $thumbnail.height() + 'px');

				$msg.removeClass(smallClass).addClass(bigClass);

				var newHeight = $thumbnail.find('p').parent().textHeight(self.Message(), $msg.width() - 10) + (self.Title().length ? $msg.find('h4').height() : 0) + 5;
				if (self.IsFile())
					newHeight += 263;

				$msg.height(newHeight + 10);

				setTimeout(function () {
					$msg.addClass('expanded').addClass('expanding');
					self.currentMessage(self.Message());
					$thumbnail.find('.icon-resize-full').removeClass('icon-resize-full').addClass('icon-resize-small');
					$thumbnail.animate({ width: $msg.width() - 10, height: newHeight }, 700, function () {
						updateGrid();
						$msg.removeClass('expanding');

						expanded(true);
					});
					if (self.IsFile()) {
						$thumbnail.find('.imgBox').animate({ height: 268 }, 700);
					}
				}, 700);

				updateGrid();

			}
			else {
				var $thumbnail = $msg.find('.thumbnail');

				var $measure = $('<li><div class="thumbnail"></div></li>').addClass(smallClass);
				$msg.parent().append($measure);
				var span4Width = $measure.find('.thumbnail').width();
				$measure.remove();

				$thumbnail
						.css('width', $thumbnail.width() + 'px')
						.css('height', $thumbnail.height() + 'px');

				var newHeight = $msg.find('p').parent().textHeight(self.smallMessage(), span4Width) + (self.Title().length ? $msg.find('h4').height() : 0) + 5;
				if (self.IsFile())
					newHeight += 115;

				$msg.addClass('expanding');
				$thumbnail.find('.icon-resize-small').removeClass('icon-resize-small').addClass('icon-resize-full');
				$thumbnail.animate({ width: span4Width, height: newHeight }, 700, function () {
					$msg.addClass(smallClass).removeClass(bigClass);
					self.currentMessage(self.smallMessage());
					$msg.css('height', $thumbnail.outerHeight() + 'px');
					$msg.removeClass('expanding');

					updateGrid();

					expanded(false);
				});
				if (self.IsFile()) {
					$thumbnail.find('.imgBox').animate({ height: 120 }, 700);
				}
				$msg.removeClass('expanded');
			}
		}
	});
	this.toggleExpanded = function () {
		self.isExpanded(!self.isExpanded());

		return false;
	};
}