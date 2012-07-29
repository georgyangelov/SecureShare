function SubscribeChannelPanel() {
	var self = this;

	this.Name = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please enter the channel name"
		}
	});
	this.Password = ko.observable("").extend({
		validation: {
			required: true
		}
	});

	this.isFormValid = ko.computed(function () {
		return this.Name.isValid() && this.Password.isValid();
	}, this);

	this.error = ko.observable();
	
	this.clear = function () {
		self.Name.reset();
		self.Password.reset();
	};

	self.submit = function () {

		amplify.request({
			resourceId: "subscribeToChannel",
			data: {
				channelName: self.Name(),
				ChannelKey: self.Password(),
				SessionKey: Application.user().SessionKey.Key()
			},
			success: function (data) {
				self.error(null);

				$('#subscribetochannel').modal('hide');
				Application.alerts.push({ type: "success", title: "Nice!", text: "You have subscribed to a new channel." });

				Application.UpdateUserInfo();

				window.location.hash = self.Name();
			},
			error: function (data, status) {
				if (data != null && typeof data.error !== "undefined" && typeof data.message !== "undefined") {
					self.error(data.message);
				}
				else {
					self.error("Something went completely wrong. Did you just divide by zero?");
				}
			}
		});

		return false;
	}
}