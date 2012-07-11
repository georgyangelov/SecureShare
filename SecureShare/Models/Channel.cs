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
		public string Name { get; set; }
		[BsonRequired]
		public string UniqueName { get; set; }

		public string Description { get; set; }

		[Required]
		[JsonIgnore]
		[BsonRequired]
		public string Password { get; set; }
		[Required]
		[JsonIgnore]
		[BsonRequired]
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
			return Regex.Replace(Name.ToLower(), @"[^a-z0-9\-]", "");
		}

		public string UpdateUniqueName()
		{
			UniqueName = GetUniqueName();

			return UniqueName;
		}
	}
}