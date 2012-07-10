using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using MongoDB.Driver.Builders;

namespace ShareGrid.Models
{
	[ModelBinder(typeof(ShareGrid.Helpers.ModelBinders.AuthenticatedRequestProvider))]
	public class AuthenticatedRequest<TData>
	{
		public TData Data { get; set; }

		public AuthType AuthType { get; set; }
		public string Key { get; set; }

		public AuthenticatedRequest()
		{

		}

		public User VerifySessionKey()
		{
			if (AuthType != Models.AuthType.SessionKey || Key == null)
				return null;

			var users = MongoDBHelper.database.GetCollection<User>("users");
			var query = Query.EQ("SessionKeys.Key", Key);

			var user = users.FindOne(query);

			if (user == null)
				return null;

			var key = (from k in user.SessionKeys
					  where k.Key == Key
					  select k).First();

			if (key.Expires < DateTime.Now)
				return null;

			return user;
		}

		public bool VerifyChannelPassword()
		{
			if (AuthType != Models.AuthType.SessionKey && Key != null)
				return false;

			throw new NotImplementedException();
		}

		public bool Verify()
		{
			return VerifySessionKey() != null || VerifyChannelPassword();
		}
	}

	public enum AuthType
	{
		SessionKey,
		ChannelPassword,
		NotAuthenticated
	}
}