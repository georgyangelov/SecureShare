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
		public AccessLevel Access { get; set; }

		public bool HasAccess(AccessLevel level)
		{
			return HasAccess(Access, level);
		}

		public static bool HasAccess(AccessLevel channelAccess, AccessLevel level)
		{
			if (level == AccessLevel.None)
				return channelAccess == AccessLevel.None;
			else if (level == AccessLevel.Normal)
				return channelAccess == AccessLevel.Normal || channelAccess == AccessLevel.Admin;
			else
				return channelAccess == AccessLevel.Admin;
		}
	}

	public enum AccessLevel
	{
		Admin = 0,
		Normal = 1,
		None = 2
	}
}