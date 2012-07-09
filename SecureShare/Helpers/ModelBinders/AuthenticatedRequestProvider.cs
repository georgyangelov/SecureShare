using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.Http.Controllers;

namespace ShareGrid.Helpers.ModelBinders
{
	public class AuthenticatedRequestProvider
		: ModelBinderProvider
	{
		AuthenticatedRequest binder = new AuthenticatedRequest();
		public override IModelBinder GetBinder(HttpActionContext actionContext, ModelBindingContext bindingContext)
		{
			if (bindingContext.ModelType.GetGenericTypeDefinition() == typeof(Models.AuthenticatedRequest<>))
			{
				return binder;
			}

			return null;
		}
	}
}