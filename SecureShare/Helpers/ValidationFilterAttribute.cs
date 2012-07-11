using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using ShareGrid.Models.Errors;
using System.Net;
using System.Web.Http;
using System.Net.Http;

namespace ShareGrid.Helpers
{
	public class ValidationFilterAttribute
		: ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var modelState = actionContext.ModelState;
			if (!modelState.IsValid)
			{
				List<ValidationProperty> errors = new List<ValidationProperty>();
				foreach (var key in modelState.Keys)
				{
					var state = modelState[key];
					if (state.Errors.Any())
					{
						errors.Add(new ValidationProperty() { name = key.Substring(key.LastIndexOf('.') + 1), message = state.Errors.First().ErrorMessage });
					}
				}

				throw new HttpResponseException(actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new InvalidParameters(errors.ToArray())));
			}
		}
	}
}