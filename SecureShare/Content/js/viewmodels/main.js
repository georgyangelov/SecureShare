function ViewModel() {
	var self = this;

    /* Globals */
	this.isLoggedIn = ko.observable(false);
	this.user = ko.observable(null);

	this.alerts = ko.observableArray([]).extend({ defaultItem: { title: "", text: "", type: "info" } });

	//self.pubnub = 

	/* Components */
	this.registerPanel = ko.observable(new RegisterPanel());
	this.loginPanel = ko.observable(new LoginPanel());
	this.userEmailPanel = ko.observable(new UserEmailPanel());
	this.userPasswordPanel = ko.observable(new UserPasswordPanel());
	this.registerChannelPanel = ko.observable(new RegisterChannelPanel());
	this.subscribeChannelPanel = ko.observable(new SubscribeChannelPanel());

	this.updateChannelInfoPanel = ko.observable();
	this.updateChannelPasswordPanel = ko.observable();
	this.updateChannelAdminPasswordPanel = ko.observable();

	/* Pages */
	this.homeView = ko.observable(null);
	this.channelView = ko.observable(null);

	/* Methods */
	this.ClearPageData = function () {
		this.homeView(null);
		this.channelView(null);
	};

	this.LogOut = function () {
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

	this.UpdateUserInfo = function (userId, sessionKey, callback) {
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

				if (typeof callback !== "undefined") {
					callback();
				}
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "We couldn't log you in. Please try logging in again" });
			}
		});
	};

	// Initialize pubnub
	this.pubnub = PUBNUB.init({
		subscribe_key: 'sub-1ca14938-da3a-11e1-a5cb-ef7f404d78f2',
		origin: 'pubsub.pubnub.com',
		ssl: true
	});

	this.init = function () {
		Sammy(function () {
			this.get('/#home', function () {
				self.ClearPageData();
				// Home page
				self.homeView({});
			});

			this.get('/#:channel', function () {
				self.ClearPageData();
				// Channel page view for this.params.channel
				self.channelView(new ChannelView(this.params.channel));
			});

			this.get('/', function () {
				this.app.runRoute('get', '#home');
			});
		}).run();
	};
}

Application = new ViewModel();
$(function () {
	ko.applyBindings(Application);

	// Fetch logged in user
	var userId = $.cookie('userId');
	var sessionKey = $.cookie('sessionKey');
	if (sessionKey != null && userId != null) {
		Application.UpdateUserInfo(userId, sessionKey, Application.init);
	}
	else {
		Application.init();
	}

	// Remove closed alerts from the Application.alerts array
	$('#alertsContainer .alert').live('closed', function () {
		Application.alerts.remove(ko.dataFor(this));
	});
});