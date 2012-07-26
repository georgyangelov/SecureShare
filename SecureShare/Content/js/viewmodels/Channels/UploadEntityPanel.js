function UploadEntityPanel(channel) {
	var self = this;

	/* Properties */
	this.Message = ko.observable("");
	this.Title = ko.observable("").extend({
		validation: {
			required: false,
			message: "Please fill at least title or message",
			func: function (value) {
				return $.trim(self.Message()) != "" || $.trim(value) != "";
			},
			computed: true
		}
	});

	this.Link = ko.observable("").extend({
		validation: {
			required: false,
			message: "The link is invalid",
			regex: /^https?\:\/\/(.*?)\.(.*?)/i
		}
	});

	/* Helpers */
	this.hasTitleOrMessage = ko.computed(function () {
		return $.trim(self.Title()) != "" || $.trim(self.Message()) != "";
	});

	this.selectedFile = ko.observable(null);
	this.hasSelectedFile = ko.computed(function () {
		return self.selectedFile() != null;
	});

	/* Methods */
	this.fileSelected = function (e, data) {
		var file = data.files[0];

		file.formattedSize = toReadableFileSize(file.size);

		self.selectedFile(file);
	};

	this.SubmitFile = function () {
		//TODO: Actual file upload to 'resourceId: "uploadEntity"'
	};

	this.SubmitMessage = function () {
		amplify.request({
			resourceId: "uploadEntity",
			data: {
				channelName: channel.Name(),
				SessionKey: Application.user().SessionKey.Key(),
				Title: self.Title(),
				Message: self.Message()
			},
			success: function (data) {
				Application.alerts.push({ type: "success", title: "Wohoo!", text: "You uploaded a message to " + channel.Name() });

				channel.hideUploadEntityPanel();
				channel.loadEntities();
			},
			error: function (data) {
				Application.alerts.push({ type: "error", title: "Oops!", text: "Something went completely wrong. Please try again in a few moments" });
			}
		});

		return false;
	};

	this.SubmitLink = function () {
		amplify.request({
			resourceId: "uploadEntity",
			data: {
				channelName: channel.Name(),
				SessionKey: Application.user().SessionKey.Key(),
				Title: self.Title(),
				Message: self.Message(),
				Link: self.Link()
			},
			success: function (data) {
				Application.alerts.push({ type: "success", title: "Wohoo!", text: "You uploaded a link to " + channel.Name() });

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