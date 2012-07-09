﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver.Builders;

namespace ShareGrid.Models
{
	public class MongoDBHelper
	{
		public static MongoServer server;
		public static MongoDatabase database;

		public static void Initialize()
		{
			var url = new MongoUrl(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
			server = MongoServer.Create(url);
			database = server.GetDatabase(url.DatabaseName, new SafeMode(true));

			EnsureIndexes();
		}

		private static void EnsureIndexes()
		{
			database.GetCollection("users")
				.EnsureIndex(IndexKeys.Ascending("Email", "SessionKey.Key"), IndexOptions.SetUnique(true));
		}

		public static string GetRandomSalt()
		{
			return Hash(Guid.NewGuid().ToString() + new Random().NextDouble() + DateTime.Now.ToString());
		}

		public static string Hash(string str)
		{
			var sha256 = new SHA256CryptoServiceProvider();
			
			return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "").ToLower();
		}

		public static string Hash(string str, string salt)
		{
			var sha256 = new SHA256CryptoServiceProvider();

			return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(salt + "_!@#$_" + Hash(str) + "_" + salt + "@#$$%"))).Replace("-", "").ToLower();
		}
	}
}