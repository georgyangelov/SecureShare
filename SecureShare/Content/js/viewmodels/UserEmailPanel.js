function UserEmailPanel() {
	var self = this;

	this.Email = ko.observable("").extend({
		validation: {
			required: true,
			regex: /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/,
			message: "Please enter your new email"
		}
	});
	this.EmailRepeat = ko.observable("");
	this.EmailRepeat = this.EmailRepeat.extend({
		validation: {
			required: true,
			message: "Please repeat your new email",
			computed: true,
			func: function (value) {
				if (value != self.Email())
					return "The two emails don't match";
			}
		}
	});

	this.isFormValid = ko.computed(function () {
		return this.Email.isValid() && this.EmailRepeat.isValid();
	}, this);

	this.error = ko.observable();

	self.submit = function () {

		amplify.request({
			resourceId: "updateUser",
			data: {
				Email: self.Email()
			},
			success: function (data) {
				self.error(null);
				self.Email.reset();
				self.EmailRepeat.reset();

				$('#userEmail').modal('hide');
				Application.alerts.push({ type: "success", text: "Your email was successfully changed." });

				var options = { path: '/' };
				if (self.RememberMe()) {
					var expiration = new Date(data.SessionKey.Expires);
					options.expires = Math.round((expiration.getTime() - new Date().getTime()) / (24 * 60 * 60 * 1000));
				}

				$.cookie('userId', data.Id, options);
				$.cookie('sessionKey', data.SessionKey.Key, options);
				Application.user(data);
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