using System.Web;
using System.Web.Mvc;
using ShareGrid.Helpers;

namespace ShareGrid
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}