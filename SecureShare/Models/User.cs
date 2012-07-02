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

namespace ShareGrid.Models
{
	public class User
		: ICanBeValidated
	{
		[BsonId(IdGenerator=typeof(StringObjectIdGenerator))]
		public string Id { get; set; }

		[BsonRequired]
		public string FirstName { get; set; }
		[BsonRequired]
		public string LastName { get; set; }

		[BsonRequired]
		public string Email { get; set; }
	
		[BsonRequired]
		[JsonIgnore]
		public string Password { get; set; }
		[BsonRequired]
		[JsonIgnore]
		public string Salt { get; set; }

		[JsonIgnore]
		[BsonIgnoreIfNull]
		public IList<SessionKey> SessionKeys { get; set; }

		[BsonIgnore]
		public SessionKey SessionKey { get; set; }

		public ValidationProperty[] Validate()
		{
			return ValidationHelper.Validate(
				new NotNullValidation("FirstName", FirstName),
				new NotNullValidation("LastName", LastName),
				new NotNullValidation("Email", Email),
				new EmailValidation  ("Email", Email),
				new NotNullValidation("Password", Password)
			);
		}
	}
}