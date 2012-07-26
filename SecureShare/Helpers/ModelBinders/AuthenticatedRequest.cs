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
using System.Web.Http;
using System.Net;
using ShareGrid.Models.Errors;

namespace ShareGrid.Helpers.ModelBinders
{
	public class AuthenticatedRequest
		: IModelBinder
	{
		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			var modelType = bindingContext.ModelType.GetGenericArguments()[0];
			var properties = modelType.GetProperties();

			var wrapperModel = Activator.CreateInstance(bindingContext.ModelType);
			object model = Activator.CreateInstance(modelType);

			NameValueCollection query;
			if (actionContext.Request.Method == HttpMethod.Get)
			{
				query = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query);
			}
			else if (actionContext.Request.Content.IsMimeMultipartContent())
			{
				var provider = new MultipartFormDataStreamProvider(System.IO.Path.GetTempPath());
				var task = actionContext.Request.Content.ReadAsMultipartAsync(provider);
				task.Wait();

				if (!task.IsCompleted)
					return false;

				query = new NameValueCollection();
				foreach (var content in task.Result)
				{
					if (content.Headers.ContentType != null && content.Headers.ContentType.MediaType != "text/plain")
						continue;

					query.Add(content.Headers.ContentDisposition.Name.Replace("\"", ""), content.ReadAsStringAsync().Result);
				}

				var filesProperty = modelType.GetProperty("FileUploads", typeof(IDictionary<string, string>));
				if (filesProperty != null)
				{
					filesProperty.SetValue(model, provider.BodyPartFileNames, null);
				}
			}
			else
			{
				Task<string> queryStringTask = actionContext.Request.Content.ReadAsStringAsync();
				queryStringTask.Wait();

				if (!queryStringTask.IsCompleted)
					return false;

				query = HttpUtility.ParseQueryString(queryStringTask.Result);
			}

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

			foreach (var property in properties)
			{
				if (query.AllKeys.Contains(property.Name))
				{
					if (property.PropertyType.IsEnum)
					{
						try
						{
							property.SetValue(model, Convert.ChangeType(query.Get(property.Name), property.PropertyType.GetEnumUnderlyingType()), null);
						}
						catch (ArgumentException)
						{
							throw new HttpResponseException(actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new InvalidParameters(new ValidationProperty() { name = property.Name, message = "Invalid enum value" })));
						}
					}
					else if (property.PropertyType == typeof(int))
					{
						try
						{
							property.SetValue(model, int.Parse(query.Get(property.Name)), null);
						}
						catch (FormatException)
						{
							throw new HttpResponseException(actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new InvalidParameters(new ValidationProperty() { name = property.Name, message = "Must be integer" })));
						}
					}
					else if (property.PropertyType == typeof(string))
					{
						property.SetValue(model, query.Get(property.Name), null);
					}
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