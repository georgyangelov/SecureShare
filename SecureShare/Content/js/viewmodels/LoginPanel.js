function RegisterPanel() {
	var self = this;

	this.Email = ko.observable("").extend({
		validation: {
			required: true,
			regex: /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/,
			message: "Please enter your email"
		}
	});
	this.Password = ko.observable("").extend({
		validation: {
			required: true,
			message: "Please enter your password"
		}
	});

	this.isFormValid = ko.computed(function () {
		return this.Email.isValid() && this.Password.isValid();
	}, this);

	this.error = ko.observable();

	self.submit = function () {

		amplify.request({
			resourceId: "login",
			data: {
				Email: self.Email(),
				Password: self.Password()
			},
			success: function (data) {
				self.error(null);

				$('#login').modal('hide');
				Application.alerts.push({ type: "success", title: "Yay!", text: "You have successfully logged in." });
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