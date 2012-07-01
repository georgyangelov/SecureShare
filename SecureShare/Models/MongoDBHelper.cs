using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ShareGrid.Models
{
	public class MongoDBHelper
	{
		public static MongoServer server;
		public static MongoDatabase database;

		public static void Initialize()
		{
			server = MongoServer.Create(ConfigurationManager.AppSettings["MongoDBConnectionString"]);
			database = server.GetDatabase(ConfigurationManager.AppSettings["MongoDBDatabase"]);
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

			return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(salt + "_!@#$_" + str + "_" + salt + "@#$$%"))).Replace("-", "").ToLower();
		}
	}
}