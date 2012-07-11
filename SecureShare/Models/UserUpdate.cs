using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShareGrid.Models
{
	public class UserUpdate
	{
		[StringLength(30, MinimumLength = 2, ErrorMessage = "Must be between 2 and 30 symbols")]
		public string FirstName { get; set; }

		[StringLength(30, MinimumLength = 2, ErrorMessage = "Must be between 2 and 30 symbols")]
		public string LastName { get; set; }

		[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "The email has invalid format")]
		public string Email { get; set; }

		[StringLength(50, MinimumLength = 4, ErrorMessage = "Must be between 4 and 30 symbols")]
		public string Password { get; set; }
	}
}