using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace ShareGrid.Models.Errors
{
	public class APIError
	{
		public string error { get; set; }
		public string message { get; set; }

		public APIError(string error, string message)
		{
			this.error = error;
			this.message = message;
		}
	}
}