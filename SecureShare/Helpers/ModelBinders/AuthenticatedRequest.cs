﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;
using System.Threading.Tasks;
using System.Collections.Specialized;
using ShareGrid.Models;

namespace ShareGrid.Helpers.ModelBinders
{
	public class AuthenticatedRequest
		: IModelBinder
	{
		public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			Task<string> queryStringTask = actionContext.Request.Content.ReadAsStringAsync();
			queryStringTask.Wait();

			if (!queryStringTask.IsCompleted)
				return false;
			
			NameValueCollection query = HttpUtility.ParseQueryString(queryStringTask.Result);

			var modelType = bindingContext.ModelType.GetGenericArguments()[0];
			var properties = modelType.GetProperties();

			var wrapperModel = Activator.CreateInstance(bindingContext.ModelType);

			if (query.AllKeys.Contains("SessionKey"))
			{
				bindingContext.ModelType.GetProperty("AuthType").SetValue(wrapperModel, AuthType.SessionKey, null);
				bindingContext.ModelType.GetProperty("Key").SetValue(wrapperModel, query.Get("SessionKey"), null);
				query.Remove("SessionKey");
			}
			else if (query.AllKeys.Contains("ChannelKey"))
			{
				bindingContext.ModelType.GetProperty("AuthType").SetValue(wrapperModel, AuthType.ChannelPassword, null);
				bindingContext.ModelType.GetProperty("Key").SetValue(wrapperModel, query.Get("ChannelKey"), null);
				query.Remove("ChannelKey");
			}
			else
			{
				bindingContext.ModelType.GetProperty("AuthType").SetValue(wrapperModel, AuthType.NotAuthenticated, null);
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

			return true;
		}
	}
}