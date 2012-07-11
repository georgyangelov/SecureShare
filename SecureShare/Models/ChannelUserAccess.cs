using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace ShareGrid.Models
{
	public class ChannelUserAccess
	{
		[BsonRequired]
		public string Id { get; set; }
		[BsonRequired]
		public UserAccess Access { get; set; }

		public bool HasAccess(UserAccess level)
		{
			if (level == UserAccess.Banned)
				return Access == UserAccess.Banned;
			else if (level == UserAccess.Normal)
				return Access == UserAccess.Normal || Access == UserAccess.Admin;
			else
				return Access == UserAccess.Admin;
		}
	}

	public enum UserAccess
	{
		Admin,
		Normal,
		Banned
	}
}