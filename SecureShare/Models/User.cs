using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using ShareGrid.Helpers;

namespace ShareGrid.Models
{
	public class User
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonRequired]
		public string FirstName { get; set; }
		[BsonRequired]
		public string LastName { get; set; }

		[BsonRequired]
		public string Email { get; set; }
		[BsonRequired]
		public string Password { get; set; }
		[BsonRequired]
		public string Salt { get; set; }

		public ValidationProperty[] Validate()
		{
			return ValidationHelper.Validate(
				new NotNullValidation("FirstName", FirstName),
				new NotNullValidation("LastName", LastName),
				new NotNullValidation("Email", Email),
				new EmailValidation("Email", Email),
				new NotNullValidation("Password", Password)
			);
		}
	}
}