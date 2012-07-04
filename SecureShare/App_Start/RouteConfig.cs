using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SecureShare.Controllers;
using ShareGrid.Helpers;

namespace ShareGrid
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			/*routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}", // {id},
				defaults: new { action = "index" }
			);*/
			routes.MapHttpRoute<UsersController>("api/users");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}