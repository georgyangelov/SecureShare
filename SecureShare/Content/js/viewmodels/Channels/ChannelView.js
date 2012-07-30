function ChannelView(channelName) {
	var self = this;

	/* Properties */
	this.Name = ko.observable(channelName);
	this.Description = ko.observable("Loading channel data...");

	this.Entities = ko.observableArray([]);

	this.UniqueName = ko.computed(function () {
		return self.Name().toLowerCase().replace(/[^a-z0-9\-]/i, '');
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

	this.loadEntities = function () {
		//TODO: Load entities in portions with infinite scroll
		var dataMappingOptions = {
			/*key: function (data) {
				return data.Id;
			},*/
			create: function (options) {
				return new ChannelEntity(options.data);
			}
		};

		amplify.request({
			resourceId: "getEntities",
			data: {
				channelName: self.UniqueName(),
				SessionKey: Application.user().SessionKey.Key(),
				start: 0,
				limit: 0
			},
			success: function (data) {
				ko.mapping.fromJS(data, dataMappingOptions, self.Entities);

				self.updateGrid();
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "We couldn't contact the server. Sorry about that!" });
			}
		});
	};

	this.loadInfo();
	this.loadEntities();
}