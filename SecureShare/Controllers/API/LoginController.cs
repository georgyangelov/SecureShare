using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShareGrid.Models;
using MongoDB.Driver.Builders;
using ShareGrid.Models.Errors;

namespace ShareGrid.Controllers.API
{
    public class LoginController : ApiController
    {
        public User Post(HttpRequestMessage request, UserLogin loginData)
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
    }
}
