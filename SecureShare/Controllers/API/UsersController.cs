using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;

namespace SecureShare.Controllers
{
	public class UsersController : ApiController
	{
		// GET api/values/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		public void Post(string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}
}
