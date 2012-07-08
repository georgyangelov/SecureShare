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
		// POST api/users
		[System.Web.Http.HttpPost]
		public HttpResponseMessage index(HttpRequestMessage request, User user)
		{
			ValidationHelper.EnsureValidity(request, user);

			var users = MongoDBHelper.database.GetCollection<User>("users");

			user.Email = user.Email.ToLower();

			if (users.FindOne(Query.EQ("Email", user.Email)) != null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Conflict, new APIError("duplicateEmail", "Duplicate email")));
			}

			user.Salt = MongoDBHelper.GetRandomSalt();
			user.Password = MongoDBHelper.Hash(user.Password, user.Salt);

			users.Insert(user);

			return new HttpResponseMessage(HttpStatusCode.Created);
		}

		// POST api/users/login
		[System.Web.Http.HttpGet]
		[Route(Uri = "login")]
		public User login(HttpRequestMessage request, UserLogin loginData)
        {
			var users = MongoDBHelper.database.GetCollection<User>("users");
			var query = Query.EQ("Email", loginData.Email);
			var user  = users.FindOne(query);

			if (user == null || user.Password != MongoDBHelper.Hash(loginData.Password, user.Salt))
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Conflict, new APIError("invalidEmailOrPassword", "Invalid email or password")));
			}

			if (user.SessionKeys == null)
				user.SessionKeys = new List<SessionKey>();

			var key = new SessionKey(user, DateTime.Now.AddDays(7));
			user.SessionKeys.Add(key);

			users.Save(user);

			user.SessionKey = key;
			
			return user;
        }

		[System.Web.Http.HttpGet]
		[Route(Uri = "checkEmail/{Email}")]
		public object checkEmail(string Email)
		{
			var users = MongoDBHelper.database.GetCollection<User>("users");
			var query = Query.EQ("Email", Email);
			var user = users.FindOne(query);

			if (user == null)
				return new { available = true };
			else
				return new { available = false };
		}
	}
}
