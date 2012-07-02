function RegisterPanel() {
	var self = this;

	this.FirstName = ko.observable("");
	this.LastName = ko.observable("");
	this.Email = ko.observable("");
	this.Password = ko.observable("");
	this.repeatPassword = ko.observable("");

	this.error = ko.observable();

	this.passwordsMatch = ko.computed(function () {
		return this.repeatPassword() == this.Password();
	}, this);

	self.register = function () {

		amplify.request({
			resourceId: "register",
			data: {
				FirstName: self.FirstName(),
				LastName: self.LastName(),
				Email: self.Email(),
				Password: self.Password()
			},
			success: function (data) {
				self.error(null);

				console.log("Register success");
			},
			error: function (data) {
				self.error(ko.mapping.fromJS(data, self.error));
				console.log("Error", data);
			}
		});

		return false;
	}
}

function ViewModel() {
	var self = this;

	self.isLoggedIn = ko.observable(false);
	self.user = ko.observable();

	/* Components */
	self.registerPanel = ko.observable(new RegisterPanel());
}

$(function () {
	ko.applyBindings(new ViewModel());
});












function updateGrid() {
	$('.thumbnails').trigger('resize');
}

function initGrid($) {
	$('.thumbnails').masonry({
		itemSelector: '.span',
		isAnimated: true,
		columnWidth: function (containerWidth) {
			return containerWidth / 6;
		}
	});

	// Fix for the script in Firefox 13. It seems to need to recalculate something
	updateGrid();
}

jQuery(initGrid);

var testText = 'Message text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about hereMessage text right about here Message text right about here Message text right about here Message text right about here';
$('.message.active:not(.expanded)').live('click', function () {
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

		setTimeout(function () {
			$msg.addClass('expanded');
			$thumbnail.find('p').text(text);
			$thumbnail.find('h4 .icon-resize-full').removeClass('icon-resize-full').addClass('icon-resize-small');
			$thumbnail.animate({ width: $msg.width() - 10, height: newHeight }, 700, function () {
				updateGrid();
			});
		}, 700);

		updateGrid();
	}
	//else {
	//	Handled by the icon click event
	//}
});
$('.message.expanded a.collapseBtn').live('click', function () {
	var text = '';
	$msg = $(this).parents('.message');
	var $thumbnail = $msg.find('.thumbnail');

	var $measure = $('<li><div class="thumbnail"></div></li>').addClass('span4');
	$msg.parent().append($measure);
	var span4Width = $measure.find('.thumbnail').width();
	$measure.remove();

	text = testText.cutToWord(200, ' ...');

	$thumbnail
			.css('width', $thumbnail.width() + 'px')
			.css('height', $thumbnail.height() + 'px');

	var newHeight = $msg.find('p').parent().textHeight(text, span4Width) + $msg.find('h4').height();

	$thumbnail.find('h4 .icon-resize-small').removeClass('icon-resize-small').addClass('icon-resize-full');
	$thumbnail.animate({ width: span4Width, height: newHeight }, 700, function () {
		$msg.addClass('span4').removeClass('span8');
		$(this).find('p').text(text);
		$msg.css('height', $thumbnail.outerHeight() + 'px');

		updateGrid();
	});
	$msg.removeClass('expanded');
});