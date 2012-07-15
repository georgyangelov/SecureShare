using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ShareGrid.Helpers
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	sealed class Route : Attribute
	{
		public Route()
		{
		}

		public string Uri { get; set; }
	}

	public static class RouteExtensions
	{
		public static void MapHttpRoute<T>(this RouteCollection routes, string prefix) where T : ApiController
		{
			Type type = typeof(T);

			var methods = from method in type.GetMethods()
						  where method.IsPublic
						  select method;

			foreach (var method in methods)
			{
				var attributes = method.GetCustomAttributes(false);
				Route route = attributes.OfType<Route>().FirstOrDefault();

				string routeUri = "";
				if (route != null)
				{
					routeUri = route.Uri;
				}

				string controllerName = type.Name.Substring(0, type.Name.LastIndexOf("Controller"));
				string routePath = prefix + "/" + routeUri;

				bool isGet = attributes.OfType<HttpGetAttribute>().FirstOrDefault() != null;
				bool isPost = attributes.OfType<HttpPostAttribute>().FirstOrDefault() != null;
				bool isPut = attributes.OfType<HttpPutAttribute>().FirstOrDefault() != null;
				bool isDelete = attributes.OfType<HttpDeleteAttribute>().FirstOrDefault() != null;
				bool isHead = attributes.OfType<HttpHeadAttribute>().FirstOrDefault() != null;

				List<string> httpMethods = new List<string>();
				if (isGet)
					httpMethods.Add("GET");
				if (isPost)
					httpMethods.Add("POST");
				if (isPut)
					httpMethods.Add("PUT");
				if (isDelete)
					httpMethods.Add("DELETE");
				if (isHead)
					httpMethods.Add("HEAD");

				object httpMethodConstraint;
				if (httpMethods.Count > 0)
					httpMethodConstraint = new { httpMethod = new HttpMethodConstraint(httpMethods.ToArray()) };
				else
					httpMethodConstraint = new { };

				// More unique route name
				string routeName = controllerName + "-" + prefix + "-" + method.Name;
				foreach (var param in method.GetParameters())
				{
					routeName += ";" + param.Name + "," + (param.IsOptional ? '1' : '0');
				}

				routes.MapHttpRoute(
					name: routeName,
					routeTemplate: routePath,
					defaults: new { controller = controllerName, action = method.Name },
					constraints: httpMethodConstraint
				);
			}
		}
	}
}