using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using ShareGrid.Helpers;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.IdGenerators;
using System.Web.Mvc;

namespace ShareGrid.Models
{
	[Bind(Exclude="Id,Salt,SessionKey,SessionKeys")]
	public class User
	{
		[BsonId(IdGenerator=typeof(StringObjectIdGenerator))]
		public string Id { get; set; }

		[BsonRequired]
		[Required(ErrorMessage = "Cannot be empty or null")]
		[StringLength(30, MinimumLength = 2, ErrorMessage = "Must be between 2 and 30 symbols")]
		public string FirstName { get; set; }

		[BsonRequired]
		[Required(ErrorMessage = "Cannot be empty or null")]
		[StringLength(30, MinimumLength = 2, ErrorMessage = "Must be between 2 and 30 symbols")]
		public string LastName { get; set; }

		[BsonRequired]
		[Required(ErrorMessage = "Cannot be empty or null")]
		[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "The email has invalid format")]
		public string Email { get; set; }
	
		[BsonRequired]
		[JsonIgnore]
		[Required(ErrorMessage = "Cannot be empty or null")]
		[StringLength(50, MinimumLength = 5, ErrorMessage = "Must be between 5 and 30 symbols")]
		public string Password { get; set; }

		[BsonRequired]
		[JsonIgnore]
		public string Salt { get; set; }

		[JsonIgnore]
		[BsonIgnoreIfNull]
		public IList<SessionKey> SessionKeys { get; set; }

		[BsonIgnore]
		public SessionKey SessionKey { get; set; }

		[BsonIgnore]
		public IList<ChannelUserAccess> Channels { get; set; }
	}
}