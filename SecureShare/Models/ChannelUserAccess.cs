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
			if (level == AccessLevel.None)
				return Access == AccessLevel.None;
			else if (level == AccessLevel.Normal)
				return Access == AccessLevel.Normal || Access == AccessLevel.Admin;
			else
				return Access == AccessLevel.Admin;
		}
	}

	public enum AccessLevel
	{
		Admin = 0,
		Normal = 1,
		None = 2
	}
}