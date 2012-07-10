function UserPasswordPanel() {
	var self = this;

	this.Password = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please enter your new password"
		}
	});
	this.PasswordRepeat = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please repeat your new password",
			computed: true,
			func: function (value) {
				if (value != self.Password())
					return "The passwords don't match";
			}
		}
	});

	this.isFormValid = ko.computed(function () {
		return this.Password.isValid() && this.PasswordRepeat.isValid();
	}, this);

	this.error = ko.observable();

	self.submit = function () {
		amplify.request({
			resourceId: "updateUser",
			data: {
				Password: self.Password(),
				SessionKey: Application.user().SessionKey.Key()
			},
			success: function (data) {
				self.error(null);
				self.Password.reset();
				self.PasswordRepeat.reset();

				$('#userPassword').modal('hide');
				Application.alerts.push({ type: "success", text: "Your password was successfully changed." });

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