function UpdateChannelInfoPanel(channel) {
	var self = this;

	this.Name = ko.observable(channel.Name()).extend({
		validation: {
			required: true,
			message: "Please enter a name for the channel",
			func: function (value) {
				if (value.length < 5 || value.length > 40)
					return false;
				else if (value.toLowerCase().replace(/[^a-z0-9\-]/i, '') == channel.Name().toLowerCase().replace(/[^a-z0-9\-]/i, '')) 
					return true;
				else {
					amplify.request({
						resourceId: "checkChannelName",
						data: {
							channelName: self.Name()
						},
						success: function (data) {
							if (!data.available) {
								self.Name.invalidate("This name is already taken");
							}
						}
					});
				}
			}
		}
	});
	this.Description = ko.observable(channel.Description());

	self.submit = function () {
		amplify.request({
			resourceId: "updateChannel",
			data: {
				channelName: channel.UniqueName(),
				Name: self.Name(),
				Description: self.Description(),
				SessionKey: Application.user().SessionKey.Key()
			},
			success: function (data) {
				$('#updateChannelInfo').modal('hide');
				Application.alerts.push({ type: "success", text: "The channel was successfully modified." });

				Application.UpdateUserInfo();
				channel.Name(self.Name());
				channel.Description(self.Description());

				window.location.hash = self.Name();
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "Sorry, but something went wrong trying to modify the channel." });
			}
		});
	}
}