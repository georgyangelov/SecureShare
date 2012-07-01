using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;
using ShareGrid.Models.Errors;
using System.Web.Http;

namespace ShareGrid.Helpers
{
	public interface ICanBeValidated
	{
		ValidationProperty[] Validate();
	}

	public class ValidationHelper
	{
		public static ValidationProperty[] Validate(params ValidationProperty[] list)
		{
			List<ValidationProperty> invalid = new List<ValidationProperty>();
			for (int i = 0; i < list.Length; i ++)
			{
				if (invalid.Exists(p => p.name == list[i].name))
					continue;

				if (!list[i].IsValid())
					invalid.Add(list[i]);
			}

			return invalid.ToArray();
		}

		public static void EnsureValidity(HttpRequestMessage request, ICanBeValidated obj)
		{
			var invalid = obj.Validate();
			if (invalid.Length > 0)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, new InvalidParameters(invalid)));
			}
		}
	}

	public class ValidationProperty
	{
		public string name { get; set; }
		public string message { get; set; }

		protected object value { get; set; }

		public ValidationProperty(string name, object obj, string message = null)
		{
			this.name = name;
			this.message = message;
			this.value = obj;
		}

		public virtual bool IsValid()
		{
			return true;
		}
	}

	public class NotNullValidation
		: ValidationProperty
	{
		public NotNullValidation(string name, object value)
			: base(name, value, "Cannot be empty or null")
		{
		}

		public override bool IsValid()
		{
			return (value != null) && (value is string && (string)value != "");
		}
	}

	public class EmailValidation
		: ValidationProperty
	{
		public EmailValidation(string name, string value)
			: base(name, value, "Invalid email")
		{
		}

		public override bool IsValid()
		{
			return Regex.IsMatch((string)value, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
		}
	}
}