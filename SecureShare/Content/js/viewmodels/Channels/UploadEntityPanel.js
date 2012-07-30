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

	var selectedFileData = ko.observable(null);

	/* Methods */
	this.fileSelected = function (e, data) {
		selectedFileData(data);
		var file = data.files[0];

		file.formattedSize = toReadableFileSize(file.size);

		self.selectedFile(file);
	};

	this.SubmitFile = function () {
		var data = selectedFileData();

		data.formData = {
			channelName: channel.Name(),
			SessionKey: Application.user().SessionKey.Key(),
			Title: self.Title(),
			Message: self.Message()
		};

		self.isUploadInProgress(true);
		self.uploadStatusText("Uploading...");
		data.submit();
	};

	/* File upload progress callbacks */
	this.isUploadInProgress = ko.observable(false);
	this.uploadProgressPercent = ko.observable(0);
	this.uploadStatusText = ko.observable("");

	this.uploadProgress = function (e, data) {
		var progress = Math.round(100 * data.loaded / data.total);

		if (progress >= 95)
			self.uploadStatusText("Processing, please wait...");

		self.uploadProgressPercent(progress);
	};
	
	this.uploadDone = function (e, data) {
		self.uploadProgressPercent(100);
		self.isUploadInProgress(false);

		Application.alerts.push({ type: "success", title: "Wohoo!", text: "You uploaded a file to " + channel.Name() });

		channel.hideUploadEntityPanel();
	};

	this.uploadFailed = function (e, data) {
		self.isUploadInProgress(false);

		Application.alerts.push({ type: "error", title: "Oops!", text: "The upload failed. Are you sure the file is less than 50MB ?" });
	};
	/* END File upload progress callbacks */

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