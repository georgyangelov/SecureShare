function ViewModel() {
	var self = this;

    /* Globals */
	self.isLoggedIn = ko.observable(false);
	self.user = ko.observable(null);

	self.alerts = ko.observableArray([]).extend({ defaultItem: { title: "", text: "", type: "info" } });

	/* Components */
	self.registerPanel = ko.observable(new RegisterPanel());
	self.loginPanel = ko.observable(new LoginPanel());
	self.userEmailPanel = ko.observable(new UserEmailPanel());
	self.userPasswordPanel = ko.observable(new UserPasswordPanel());
	self.registerChannelPanel = ko.observable(new RegisterChannelPanel());
	self.subscribeChannelPanel = ko.observable(new SubscribeChannelPanel());

	/* Pages */
	self.homePage = ko.observable(null);
	self.channelViewPage = ko.observable(null);

	/* Methods */
	self.ClearPageData = function () {
		self.homePage(null);
		self.channelViewPage(null);
	};

	self.LogOut = function () {
		amplify.request({
			resourceId: "logout",
			data: {
				sessionKey: self.user().SessionKey.Key()
			}
		});

		self.isLoggedIn(false);
		self.user({});
		$.cookie('userId', null);
		$.cookie('sessionKey', null);

		return false;
	};

	self.UpdateUserInfo = function (userId, sessionKey) {
		if (typeof userId === "undefined" || typeof sessionKey === "undefined") {
			if (!self.isLoggedIn()) {
				return;
			}

			userId = self.user().Id();
			sessionKey = self.user().SessionKey.Key();
		}

		amplify.request({
			resourceId: "userLoginInfo",
			data: {
				userId: userId,
				sessionKey: sessionKey
			},
			success: function (data) {
				Application.isLoggedIn(true);
				Application.user(ko.mapping.fromJS(data));
			}
		});
	};

	Sammy(function () {
		this.get('#home', function () {
			self.ClearPageData();
			// Home page
			self.homePage({});
		});

		this.get('#:channel', function () {
			self.ClearPageData();
			// Channel page view for this.params.channel
			self.channelViewPage({});
		});

		this.get('', function () {
			this.app.runRoute('get', '#home');
		});
	}).run();
}

Application = new ViewModel();
$(function () {
	ko.applyBindings(Application);

	// Fetch logged in user
	var userId = $.cookie('userId');
	var sessionKey = $.cookie('sessionKey');
	if (sessionKey != null && userId != null) {
		Application.UpdateUserInfo(userId, sessionKey);
	}

	// Remove closed alerts from the Application.alerts array
	$('#alertsContainer .alert').live('closed', function () {
		Application.alerts.remove(ko.dataFor(this));
	});
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