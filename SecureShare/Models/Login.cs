using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SecureShare.Models
{
	public class Login
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		[Remote("EmailAvailable", "User")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}