using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ShareGrid.Models
{
	[Bind(Exclude = "Id,ChannelId,UserId,Date,Importance")]
	public class ChannelEntity
	{
		[BsonRequired]
		[BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
		public string Id { get; set; }
		[BsonRequired]
		[JsonIgnore]
		public string ChannelId { get; set; }

		public string UserId { get; set; }

		public string Title { get; set; }
		public string Message { get; set; }
		public string Link { get; set; }

		public DateTime Date { get; set; }

		public Importance Importance { get; set; }

		public void ResetEmpty()
		{
			if (Title == "")
				Title = null;
			if (Message == "")
				Message = null;
			if (Link == "")
				Link = null;
		}
	}

	public enum Importance
	{
		High = 0,
		Normal = 1,
		Low = 2
	}
}