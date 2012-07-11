using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace ShareGrid.Models
{
	[Bind(Exclude = "Salt,CreatorId,CreationDate,Users")]
	public class Channel
	{
		[BsonRequired]
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string Id { get; set; }

		[Required]
		[BsonRequired]
		[StringLength(40, MinimumLength = 5, ErrorMessage = "Must be between 5 and 40 symbols")]
		public string Name { get; set; }
		[BsonRequired]
		public string UniqueName { get; set; }

		public string Description { get; set; }

		[Required]
		[JsonIgnore]
		[BsonRequired]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "Must be between 5 and 50 symbols")]
		public string Password { get; set; }
		[Required]
		[JsonIgnore]
		[BsonRequired]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "Must be between 5 and 50 symbols")]
		public string AdminPassword { get; set; }

		[JsonIgnore]
		[BsonRequired]
		public string Salt { get; set; }

		[JsonIgnore]
		public string CreatorId { get; set; }
		public string CreationDate { get; set; }

		[BsonIgnoreIfNull]
		public IList<ChannelUserAccess> Users { get; set; }

		public string GetUniqueName()
		{
			return GetUniqueName(Name);
		}

		public string UpdateUniqueName()
		{
			UniqueName = GetUniqueName(Name);

			return UniqueName;
		}

		public static string GetUniqueName(string channelName)
		{
			return Regex.Replace(channelName.ToLower(), @"[^a-z0-9\-]", "");
		}
	}
}