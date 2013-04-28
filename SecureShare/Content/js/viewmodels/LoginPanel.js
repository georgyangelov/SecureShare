function LoginPanel() {
	var self = this;

	this.Email = ko.observable("test@test.test").extend({
		validation: {
			required: true,
			regex: /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/,
			message: "Please enter your email"
		}
	});
	this.Password = ko.observable("testtest").extend({
		validation: {
			required: true,
			message: "Please enter your password"
		}
	});
	this.RememberMe = ko.observable(false);

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

				Application.isLoggedIn(true);
				$('#login').modal('hide');
				Application.alerts.push({ type: "success", title: "Welcome back, " + data.FirstName + "!", text: "You have successfully logged in. Now you can browse through your subscribed channels using the menu on the right." });

				var options = {path: '/'};
				if (self.RememberMe()) {
					var expiration = new Date(data.SessionKey.Expires);
					options.expires = Math.round((expiration.getTime() - new Date().getTime()) / (24 * 60 * 60 * 1000));
				}

				$.cookie('userId', data.Id, options);
				$.cookie('sessionKey', data.SessionKey.Key, options);
				Application.UpdateUserInfo(data.Id, data.SessionKey.Key);
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