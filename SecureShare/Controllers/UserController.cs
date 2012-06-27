using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecureShare.Controllers
{
    public class UserController : Controller
    {
		[HttpPost]
		public ActionResult Login(Models.Login info)
		{
			try
			{
				// TODO: Add insert logic here

				return RedirectToAction("Index", "Home");
			}
			catch
			{
				return View();
			}
		}


		public bool EmailAvailable(string email)
		{
			return false;
		} 
    }
}
