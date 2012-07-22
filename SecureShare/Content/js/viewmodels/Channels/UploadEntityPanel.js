function UploadEntityPanel(channel) {
	var self = this;

	/* Properties */
	this.Title = ko.observable("");
	this.Message = ko.observable("");

	this.Link = ko.observable("").extend({
		validation: {
			required: false,
			message: "The link is invalid",
			regex: /^https?\:\/\/(.*?)\.(.*?)/i
		}
	});

	/* Methods */
	this.Submit = function () {
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