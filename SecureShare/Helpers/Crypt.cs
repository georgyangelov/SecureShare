using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ShareGrid.Helpers
{
	public class Crypt
	{
		public static string EncryptText(string text, string key)
		{
			if (text == null)
				return null;

			Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(key, HexStringToBytes(ConfigurationManager.AppSettings["ApplicationSecretKey"]));
			using (var aes = new AesCryptoServiceProvider())
			{
				aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
				aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);

				byte[] stringB = Encoding.UTF8.GetBytes(text);
				return Convert.ToBase64String(aes.CreateEncryptor().TransformFinalBlock(stringB, 0, stringB.Length));
			}
		}

		public static string DecryptText(string text, string key)
		{
			if (text == null)
				return null;

			Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(key, HexStringToBytes(ConfigurationManager.AppSettings["ApplicationSecretKey"]));
			using (var aes = new AesCryptoServiceProvider())
			{
				aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
				aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);

				byte[] stringB = Convert.FromBase64String(text);
				return Encoding.UTF8.GetString(aes.CreateDecryptor().TransformFinalBlock(stringB, 0, stringB.Length));
			}
		}



		public static byte[] HexStringToBytes(string value)
		{
			SoapHexBinary shb = SoapHexBinary.Parse(value);
			return shb.Value;
		}
	}
}