using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShareGrid.Helpers;

namespace ShareGrid.Models.Errors
{
	public class InvalidParameters
		: APIError
	{
		public ValidationProperty[] parameters { get; set; }

		public InvalidParameters(ValidationProperty[] parameters)
			: base("invalidParameters", "Some of the request parameters are invalid. Check the 'parameters' property for a list")
		{
			this.parameters = parameters;
		}
	}

	public class ValidationProperty
	{
		public string name { get; set; }
		public string message { get; set; }
	}
}