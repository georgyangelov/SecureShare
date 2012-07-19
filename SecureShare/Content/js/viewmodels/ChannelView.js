function updateGrid() {
	$('.thumbnails').masonry("reload");
}

function ChannelView() {
	var self = this;

	/* Properties */
	this.Name = ko.observable("Test channel");
	this.Description = ko.observable("Hey there!");

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
		self.uploadEntityPanel(new UploadEntityPanel(self.Name()));
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

	this.loadEntities = function () {
		//TODO
	};
}

function UploadEntityPanel(channelName) {
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
				channelName: channelName,
				sessionKey: sessionKey,
				Title: self.Title(),
				Message: self.Message(),
				Link: self.Link()
			},
			success: function (data) {
				Application.isLoggedIn(true);
				Application.user(ko.mapping.fromJS(data));
			}
		});
	};
}

$('a[data-toggle="tab"]').live('shown', function (e) {
	updateGrid();
});