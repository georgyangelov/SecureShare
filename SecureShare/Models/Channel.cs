using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;

namespace ShareGrid.Models
{
	public class Channel
	{
		[BsonRequired]
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string Id { get; set; }

		[BsonRequired]
		public string Name { get; set; }
		public string Description { get; set; }

		[JsonIgnore]
		[BsonRequired]
		public string Password { get; set; }
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
	}
}