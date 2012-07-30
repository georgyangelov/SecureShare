function UpdateChannelPasswordPanel(channel) {
	var self = this;

	this.Password = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please enter a strong channel password",
			func: function (value) {
				return value.length >= 5 && value.length <= 50;
			}
		}
	});
	this.PasswordRepeat = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please repeat the password",
			func: function (value) {
				if (self.Password() != value)
					return "The two passwords don't match";
			},
			computed: true
		}
	});

	this.isFormValid = ko.computed(function () {
		return self.Password.isValid() && self.PasswordRepeat.isValid();
	});

	self.submit = function () {
		amplify.request({
			resourceId: "updateChannel",
			data: {
				channelName: channel.UniqueName(),
				Password: self.Password(),
				SessionKey: Application.user().SessionKey.Key()
			},
			success: function (data) {
				$('#updateChannelPassword').modal('hide');
				Application.alerts.push({ type: "success", text: "The channel was successfully modified." });
			},
			error: function (data) {
				Application.alerts.push({ type: "error", text: "Sorry, but something went wrong trying to modify the channel." });
			}
		});
	}
}