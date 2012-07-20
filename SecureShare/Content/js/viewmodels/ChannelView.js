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
	this.Title = ko.observable(data.Title);
	this.Message = ko.observable(data.Message);
	this.Link = ko.observable(data.Link);

	this.Importance = ko.observable(data.Importance);

	/* Helper properties */
	this.smallMessage = ko.computed(function () {
		return self.Message().cutToWord(200, ' ...');
	});

	this.isExpandable = ko.computed(function () {
		return self.smallMessage() != self.Message();
	});

	var expanded = ko.observable(false);
	this.isExpanded = ko.computed({
		read: function () {
			return expanded();
		},
		write: function (value) {
			if (value) {
				//TODO: Expand the panel

				expanded(true);
			}
			else {
				//TODO: Contract the panel

				expanded(false);
			}
		}
	});
}

function ChannelView(channelName) {
	var self = this;

	/* Properties */
	this.Name = ko.observable(channelName);
	this.Description = ko.observable("Loading channel data...");

	this.Entities = ko.observableArray([]);

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
		self.uploadEntityPanel(new UploadEntityPanel(self));
		self.updateGrid();

		return false;
	};

	this.hideUploadEntityPanel = function () {
		self.uploadEntityPanel(null);
		self.updateGrid();

		return false;
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
				channelName: channelName,
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
				channelName: self.Name(),
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

function UploadEntityPanel(channel) {
	var self = this;

	/* Properties */
	this.Title = ko.observable("");
	this.Message = ko.observable("");

	this.Link = ko.observable("").extend({
		validation: {
			required: false,
			message: "The link is invalid",
			regex: /^https?\:\/\//i
		}
	});

	/* Methods */
	this.Submit = function () {
		amplify.request({
			resourceId: "uploadEntity",
			data: {
				channelName: channel.Name(),
				sessionKey: Application.user().SessionKey.Key(),
				Title: self.Title(),
				Message: self.Message(),
				Link: self.Link()
			},
			success: function (data) {
				Application.alerts.push({ type: "success", title: "Wohoo!", text: "You uploaded an item to " + channel.Name() });

				channel.hideUploadEntityPanel();
				channel.loadEntities();
			},
			error: function (data) {
				Application.alerts.push({ type: "error", title: "Oops!", text: "Something went completely wrong. Please try again in a few moments" });
			}
		});

		return false;
	};
}

$('a[data-toggle="tab"]').live('shown', function (e) {
	updateGrid();
});