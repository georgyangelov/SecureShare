function RegisterChannelPanel() {
	var self = this;

	this.Name = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please enter a name for the channel",
			func: function (value) {
				if (value.length < 5 || value.length > 40)
					return false;
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
	this.Description = ko.observable("");
	this.Password = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please enter a channel password",
			func: function (value) {
				return value.length >= 5 && value.length <= 50;
			}
		}
	});
	this.AdminPassword = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please enter a channel password for admins",
			func: function (value) {
				return value.length >= 5 && value.length <= 50;
			}
		}
	});
	this.AdminPasswordRepeat = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please repeat the admin password",
			func: function (value) {
				if (self.AdminPassword() != value)
					return "The two passwords don't match";
			},
			computed: true
		}
	});

	this.isFormValid = ko.computed(function () {
		return this.Name.isValid() && this.Password.isValid() && this.AdminPassword.isValid() && this.AdminPasswordRepeat.isValid();
	}, this);

	this.error = ko.observable();

	this.clear = function () {
		self.Name.reset();
		self.Description.reset();
		self.Password.reset();
		self.AdminPassword.reset();
		self.AdminPasswordRepeat.reset();
	};

	self.submit = function () {

		amplify.request({
			resourceId: "registerChannel",
			data: {
				Name: self.Name(),
				Description: self.Description(),
				Password: self.Password(),
				AdminPassword: self.AdminPassword()
			},
			success: function (data) {
				self.error(null);

				$('#registerchannel').modal('hide');
				Application.alerts.push({ type: "success", title: "Great!", text: "You have successfully registered a channel." });

				self.clear();

				Application.UpdateUserInfo();
			},
			error: function (data) {
				if (typeof data.error !== "undefined" && typeof data.message !== "undefined") {
					self.error(data.message);
				} else {
					self.error("Something went completely wrong. Did you just divide by zero?");
				}
			}
		});

		return false;
	}
}