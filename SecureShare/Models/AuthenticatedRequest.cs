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

		public User VerifySessionKey(Channel channel, UserAccess accessLevel = UserAccess.Admin)
		{
			var user = VerifySessionKey();

			if (user != null && VerifyUserAccess(user, channel, accessLevel))
				return user;
			else
				return null;
		}

		private bool VerifyUserAccess(User user, Channel channel, UserAccess accessLevel)
		{
			var userAccessLevels = from u in channel.Users
								   where u.Id == user.Id
								   select u;

			if (!userAccessLevels.Any())
				return false;

			var userAccessLevel = userAccessLevels.First();

			return userAccessLevel.HasAccess(accessLevel);
		}

		public bool VerifyChannelPassword(Channel channel, UserAccess accessLevel = UserAccess.Admin)
		{
			if (AuthType != Models.AuthType.ChannelPassword || Key != null)
				return false;

			if (accessLevel == UserAccess.Banned)
				throw new ArgumentException("accessLevel cannot be Banned for this method", "accessLevel");

			if (channel.AdminPassword == MongoDBHelper.Hash(Key, channel.Salt))
				return true;
			else if (channel.Password == MongoDBHelper.Hash(Key, channel.Salt))
				return accessLevel == UserAccess.Normal;
			else
				return false;
		}

		public bool Verify(Channel channel, UserAccess accessLevel)
		{
			return VerifySessionKey(channel, accessLevel) != null || VerifyChannelPassword(channel);
		}
	}

	public enum AuthType
	{
		SessionKey,
		ChannelPassword,
		NotAuthenticated
	}
}