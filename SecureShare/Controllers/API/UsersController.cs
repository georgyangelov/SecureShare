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
		[System.Web.Http.HttpPost]
		[Route(Uri = "login")]
		public User login(HttpRequestMessage request, UserLogin loginData)
        {
			var users = MongoDBHelper.database.GetCollection<User>("users");
			var query = Query.EQ("Email", loginData.Email);
			var user  = users.FindOne(query);

			if (user == null || user.Password != MongoDBHelper.Hash(loginData.Password, user.Salt))
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, new APIError("invalidEmailOrPassword", "Invalid email or password")));
			}

			if (user.SessionKeys == null)
				user.SessionKeys = new List<SessionKey>();

			var key = new SessionKey(user, DateTime.Now.AddDays(7));
			user.SessionKeys.Add(key);

			users.Save(user);

			user.SessionKey = key;
			
			return user;
        }

		[System.Web.Http.HttpDelete]
		[Route(Uri = "logout/{sessionKey}")]
		public SuccessReport logout(string userId, string sessionKey)
		{
			var users = MongoDBHelper.database.GetCollection<User>("users");
			var query = Query.EQ("SessionKeys.Key", sessionKey);
			var user = users.FindOne(query);

			if (user != null)
			{
				user.SessionKeys.Remove((from k in user.SessionKeys
										where k.Key == sessionKey
										select k).First());

				users.Save(user);
			}

			return new SuccessReport(true);
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

		[System.Web.Http.HttpPut]
		public void updateUser(HttpRequestMessage request, AuthenticatedRequest<UserUpdate> userInfo)
		{
			User user = userInfo.VerifySessionKey(); 
			if (user == null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidSessionKey", "Invalid, expired or non-existant session key. Please login properly")));
			}

			if (userInfo.Data.Email != null)
				user.Email = userInfo.Data.Email;
			if (userInfo.Data.FirstName != null)
				user.FirstName = userInfo.Data.FirstName;
			if (userInfo.Data.LastName != null)
				user.LastName = userInfo.Data.LastName;
			if (userInfo.Data.Password != null)
				user.Password = MongoDBHelper.Hash(userInfo.Data.Password, user.Salt);

			MongoDBHelper.database.GetCollection<User>("users").Save(user);
		}

		[System.Web.Http.HttpGet]
		[Route(Uri = "{userId}")]
		public User GetUser(HttpRequestMessage request, string userId)
		{
			var users = MongoDBHelper.database.GetCollection<User>("users");
			var user = users.FindOne(Query.EQ("_id", userId));

			if (user == null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidUserId", "Invalid or non-existant user id")));
			}

			return user;
		}

		[System.Web.Http.HttpGet]
		[Route(Uri = "{userId}/{sessionKey}")]
		public User GetUser(HttpRequestMessage request, string userId, string sessionKey)
		{
			var users = MongoDBHelper.database.GetCollection<User>("users");
			var user = users.FindOne(Query.EQ("_id", userId));

			if (user == null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidUserId", "Invalid or non-existant user id")));
			}

			var key = from k in user.SessionKeys
					  where k.Key == sessionKey && k.Expires > DateTime.Now
					  select k;

			if (key.Count() == 0)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, new APIError("invalidSessionKey", "Invalid, expired or non-existant session key")));
			}

			user.SessionKey = key.First();

			return user;
		}
	}
}
