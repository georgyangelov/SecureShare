function ChannelView(channelName) {
	var self = this;

	/* Properties */
	this.Name = ko.observable(channelName);
	this.Description = ko.observable("Loading channel data...");

	this.Entities = ko.observableArray([]);

	this.UniqueName = ko.computed(function () {
		return self.Name().toLowerCase().replace(/[^a-z0-9\-]/i, '');
	});

	/* Panels */
	this.uploadEntityPanel = ko.observable();

	/* Methods */
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
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "We couldn't contact the server. Sorry about that!" });
			}
		});
	};

	this.loadEntities = function () {
		//TODO: Load entities in portions with infinite scroll
		var dataMappingOptions = {
			key: function (data) {
				return data.Id;
			},
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