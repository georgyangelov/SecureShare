using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using System.Threading.Tasks;
using System.Collections.Specialized;
using ShareGrid.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace ShareGrid.Helpers.ModelBinders
{
	public class AuthenticatedRequest
		: IModelBinder
	{
		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			NameValueCollection query;
			if (actionContext.Request.Method == HttpMethod.Get)
			{
				query = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
			}
			else
			{
				Task<string> queryStringTask = actionContext.Request.Content.ReadAsStringAsync();
				queryStringTask.Wait();

				if (!queryStringTask.IsCompleted)
					return false;

				query = HttpUtility.ParseQueryString(queryStringTask.Result);
			}

			var modelType = bindingContext.ModelType.GetGenericArguments()[0];
			var properties = modelType.GetProperties();

			var wrapperModel = Activator.CreateInstance(bindingContext.ModelType);

			if (query.AllKeys.Contains("SessionKey"))
			{
				bindingContext.ModelType.GetProperty("SessionKey").SetValue(wrapperModel, query.Get("SessionKey"), null);
				query.Remove("SessionKey");
			}
			
			if (query.AllKeys.Contains("ChannelKey"))
			{
				bindingContext.ModelType.GetProperty("ChannelKey").SetValue(wrapperModel, query.Get("ChannelKey"), null);
				query.Remove("ChannelKey");
			}

			object model = Activator.CreateInstance(modelType);
			foreach (var property in properties)
			{
				if (query.AllKeys.Contains(property.Name))
				{
					property.SetValue(model, query.Get(property.Name), null);
				}
			}
			bindingContext.ModelType.GetProperty("Data").SetValue(wrapperModel, model, null);

			bindingContext.Model = wrapperModel;


			/* Validate */
			// DataAnnotation Validation
			var validationResult = from prop in TypeDescriptor.GetProperties(model).Cast<PropertyDescriptor>()
								   from attribute in prop.Attributes.OfType<ValidationAttribute>()
								   where !attribute.IsValid(prop.GetValue(model))
								   select new { Property = prop.Name, ErrorMessage = attribute.FormatErrorMessage(string.Empty) };

			// Add the ValidationResult's to the ModelState
			foreach (var validationResultItem in validationResult)
				bindingContext.ModelState.AddModelError(validationResultItem.Property, validationResultItem.ErrorMessage);

			return true;
		}
	}
}