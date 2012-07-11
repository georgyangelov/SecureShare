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
	}

	public enum UserAccess
	{
		Admin,
		Normal,
		Banned
	}
}