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

		public AuthType AuthType
		{
			get
			{
				if (SessionKey != null && ChannelKey != null)
					return Models.AuthType.Both;
				else if (SessionKey != null)
					return Models.AuthType.SessionKey;
				else if (ChannelKey != null)
					return Models.AuthType.ChannelPassword;
				else
					return Models.AuthType.NotAuthenticated;
			}
		}
		public string SessionKey { get; set; }
		public string ChannelKey { get; set; }

		public AuthenticatedRequest()
		{

		}

		public User VerifySessionKey()
		{
			if ((AuthType != Models.AuthType.SessionKey && AuthType != Models.AuthType.Both) || SessionKey == null)
				return null;

			var users = MongoDBHelper.database.GetCollection<User>("users");
			var query = Query.EQ("SessionKeys.Key", SessionKey);

			var user = users.FindOne(query);

			if (user == null)
				return null;

			var key = (from k in user.SessionKeys
					   where k.Key == SessionKey
					  select k).First();

			if (key.Expires < DateTime.Now)
				return null;

			return user;
		}

		public User VerifySessionKey(Channel channel, AccessLevel accessLevel = AccessLevel.Admin)
		{
			var user = VerifySessionKey();

			if (user != null && ChannelUserAccess.HasAccess(GetUserAccess(user, channel), accessLevel))
				return user;
			else
				return null;
		}

		public Tuple<User, AccessLevel> VerifySessionKey(Channel channel)
		{
			var user = VerifySessionKey();

			if (user != null)
				return new Tuple<User, AccessLevel>(user, GetUserAccess(user, channel));
			else
				return new Tuple<User, AccessLevel>(null, AccessLevel.None);
		}

		private AccessLevel GetUserAccess(User user, Channel channel)
		{
			var userAccessLevels = from u in channel.Users
								   where u.Id == user.Id
								   select u;

			if (!userAccessLevels.Any())
				return AccessLevel.None;

			var userAccessLevel = userAccessLevels.First();

			return userAccessLevel.Access;
		}

		public bool VerifyChannelPassword(Channel channel, AccessLevel accessLevel)
		{
			AccessLevel access = VerifyChannelPassword(channel);

			if (access == AccessLevel.Normal && accessLevel == AccessLevel.Normal)
				return true;
			else if (access == AccessLevel.Admin)
				return true;

			return false;
		}

		public AccessLevel VerifyChannelPassword(Channel channel)
		{
			if ((AuthType != Models.AuthType.ChannelPassword && AuthType != Models.AuthType.Both) || ChannelKey == null)
				return AccessLevel.None;

			if (channel.AdminPassword == MongoDBHelper.Hash(ChannelKey, channel.Salt))
				return AccessLevel.Admin;
			else if (channel.Password == MongoDBHelper.Hash(ChannelKey, channel.Salt))
				return AccessLevel.Normal;
			else
				return AccessLevel.None;
		}

		public bool Verify(Channel channel, AccessLevel accessLevel)
		{
			return VerifySessionKey(channel, accessLevel) != null || VerifyChannelPassword(channel, accessLevel);
		}

		public Tuple<User, AccessLevel> Verify(Channel channel)
		{
			if (AuthType == Models.AuthType.NotAuthenticated)
				return new Tuple<User, AccessLevel>(null, AccessLevel.None);

			if (AuthType == Models.AuthType.SessionKey)
				return VerifySessionKey(channel);
			else
				return new Tuple<User, AccessLevel>(null, VerifyChannelPassword(channel));
		}
	}

	public enum AuthType
	{
		Both,
		SessionKey,
		ChannelPassword,
		NotAuthenticated
	}
}