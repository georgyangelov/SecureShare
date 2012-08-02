function ChannelView(channelName) {
	var self = this;

	/* Properties */
	this.Name = ko.observable(channelName);
	this.Description = ko.observable("Loading channel data...");

	this.UnsortedEntities = ko.observableArray([]);
	this.Entities = ko.computed({
		read: function () {
			return self.UnsortedEntities().sort(function (a, b) {
				return new Date(ko.utils.unwrapObservable(b.Date)).getTime() - new Date(ko.utils.unwrapObservable(a.Date)).getTime();
			});
		},
		write: function (value) {
			self.UnsortedEntities(value);
		}
	});

	this.UniqueName = ko.computed(function () {
		return self.Name().toLowerCase().replace(/[^a-zа-я0-9\-]/ig, '');
	});

	this.isAdmin = ko.observable(false);

	/* Panels */
	this.uploadEntityPanel = ko.observable();

	/* Methods */
	this.unsubscribe = function () {
		if (!confirm("Are you sure you want to unsubscribe from " + self.Name() + "?"))
			return;

		amplify.request({
			resourceId: "unsubscribeFromChannel",
			data: {
				channelName: self.UniqueName(),
				userId: Application.user().Id(),
				SessionKey: Application.user().SessionKey.Key()
			},
			success: function (data) {
				window.location.hash = "home";
				Application.UpdateUserInfo();
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "We couldn't contact the server. Sorry about that!" });
			}
		});
	};

	this.deleteChannel = function () {
		if (!confirm("Are you sure you want to permanently remove " + self.Name() + "?"))
			return;

		amplify.request({
			resourceId: "deleteChannel",
			data: {
				channelName: self.UniqueName(),
				SessionKey: Application.user().SessionKey.Key()
			},
			success: function (data) {
				window.location.hash = "home";
				Application.UpdateUserInfo();
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "We couldn't contact the server. Sorry about that!" });
			}
		});
	};

	this.showUpdateInfoPanel = function () {
		Application.updateChannelInfoPanel(new UpdateChannelInfoPanel(this));
		$('#updateChannelInfo').modal();
	};
	this.showUpdatePasswordPanel = function () {
		Application.updateChannelPasswordPanel(new UpdateChannelPasswordPanel(this));
		$('#updateChannelPassword').modal();
	};
	this.showUpdateAdminPasswordPanel = function () {
		Application.updateChannelAdminPasswordPanel(new UpdateChannelAdminPasswordPanel(this));
		$('#updateChannelAdminPassword').modal();
	};

	this.updateGrid = function ($elements, reloadOrAppend) {
		if (typeof $elements !== "undefined") {
			if (typeof reloadOrAppend === "undefined")
				reloadOrAppend = "reload";

			$('.thumbnails').masonry(reloadOrAppend, $elements);
		}
		else
			updateGrid();
	};

	this.showUploadEntityPanel = function () {
		if (self.uploadEntityPanel() != null)
			return;

		self.uploadEntityPanel(new UploadEntityPanel(self));
		self.updateGrid();
	};

	this.hideUploadEntityPanel = function () {
		self.uploadEntityPanel(null);
		self.updateGrid();
	};

	this.toggleUploadEntityPanel = function () {
		if (self.uploadEntityPanel() == null)
			return self.showUploadEntityPanel();
		else
			return self.hideUploadEntityPanel();
	};


	function findOne(array, func) {
		for (var i = 0; i < array.length; i++) {
			if (func(array[i], i))
				return array[i];
		}

		return null;
	}

	this.loadInfo = function () {
		amplify.request({
			resourceId: "getChannelInfo",
			data: {
				channelName: self.UniqueName(),
				SessionKey: Application.user().SessionKey.Key()
			},
			success: function (data) {
				self.Name(data.Name);
				self.Description(data.Description);

				var userAccess = findOne(data.Users, function (value, index) {
					return value.Id == Application.user().Id();
				});

				if (userAccess.Access == 0)
					self.isAdmin(true);
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "We couldn't contact the server. Sorry about that!" });
			}
		});
	};

	var loading = false;
	var reachedEnd = false;
	this.loadEntities = function (offset, count, append) {
		loading = true;

		if (typeof offset === "undefined")
			offset = 0;
		if (typeof count === "undefined")
			count = 0;
		if (typeof append === "undefined")
			append = false;

		amplify.request({
			resourceId: "getEntities",
			data: {
				channelName: self.UniqueName(),
				SessionKey: Application.user().SessionKey.Key(),
				start: offset,
				limit: count
			},
			success: function (data) {

				//ko.mapping.fromJS(data, dataMappingOptions, self.Entities);
				mapArray(data, self.UnsortedEntities, function (object) {
					return ko.utils.unwrapObservable(object.Id);
				},
				function (data) {
					return new ChannelEntity(data);
				}, append);

				self.updateGrid();
				loading = false;
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "We couldn't contact the server. Sorry about that!" });
				loading = false;
			}
		});
	};

	this.loadMoreEntities = function () {
		if (loading || reachedEnd)
			return;

		var offset = self.UnsortedEntities().length;

		self.loadEntities(offset, 40, true);
	};

	this.loadInfo();
	this.loadEntities(0, 40);

	// Subscribe to the pubnub channel
	Application.pubnub.subscribe({
		channel: "channel_" + self.UniqueName(),
		restore: false,

		callback: function (message) {
			//console.log("Received a notification from pubnub", message);
			self.loadEntities(0, 2, true);
		},

		disconnect: function () {
		},

		reconnect: function () {
			self.loadEntities(0, 5, true);
		},

		connect: function () {
		}
	})
}