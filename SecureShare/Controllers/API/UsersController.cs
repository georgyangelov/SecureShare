using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using ShareGrid.Models;
using MongoDB.Driver.Builders;
using System.Net;
using System.Net.Http;
using ShareGrid.Models.Errors;
using ShareGrid.Helpers;

namespace SecureShare.Controllers
{
	public class UsersController : ApiController
	{
		// GET api/values/5
		/*public string Get(int id)
		{
			return "value";
		}*/

		// POST api/users
		public HttpResponseMessage Post(HttpRequestMessage request, User user)
		{
			ValidationHelper.EnsureValidity(request, user);

			var users = MongoDBHelper.database.GetCollection<User>("users");

			if (users.FindOne(Query.EQ("Email", user.Email)) != null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Conflict, new APIError("duplicateEmail", "Duplicate email")));
			}

			user.Salt = MongoDBHelper.GetRandomSalt();
			user.Password = MongoDBHelper.Hash(user.Password, user.Salt);

			users.Insert(user);

			return new HttpResponseMessage(HttpStatusCode.Created);
		}

		// PUT api/values/5
		/*public void Put(int id, string value)
		{
		}*/

		// DELETE api/values/5
		/*public void Delete(int id)
		{
		}*/
	}
}
