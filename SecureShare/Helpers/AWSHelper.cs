﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Amazon.S3;
using Amazon.S3.Model;

namespace ShareGrid.Helpers
{
	public class AWSHelper
	{
		public static AmazonS3 S3;
		private static byte[] appKey;

		public static void Initialize()
		{
			S3 = new AmazonS3Client(ConfigurationManager.AppSettings["AWSAccessKey"], ConfigurationManager.AppSettings["AWSSecretKey"], new AmazonS3Config());
			appKey = Crypt.HexStringToBytes(ConfigurationManager.AppSettings["ApplicationSecretKey"]);
		}

		private static void CreateBuckets()
		{
			S3.PutBucket(new PutBucketRequest()
				.WithBucketName(ConfigurationManager.AppSettings["AWSBucketName"]));
		}

		public static Stream GetDecryptedFileStream(string awsPath, string key)
		{
			var fileResponse = S3.GetObject(new GetObjectRequest()
				{
					BucketName = ConfigurationManager.AppSettings["AWSBucketName"],
					Key = awsPath
				}
			);
			Stream result = fileResponse.ResponseStream;

			if (fileResponse.Metadata.AllKeys.Contains("x-amz-meta-customencrypt") && fileResponse.Metadata["x-amz-meta-customencrypt"] == "true")
			{
				Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(key, appKey);
				using (var aes = new AesCryptoServiceProvider())
				{
					aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
					aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);
					result = new CryptoStream(result, aes.CreateDecryptor(), CryptoStreamMode.Read);
				}
			}

			return new HoldReferenceStream<GetObjectResponse>(result, fileResponse);
		}

		public static void UploadFile(string file, string awsPath)
		{
			PutObjectRequest request = new PutObjectRequest()
				{
					FilePath = file,
					BucketName = ConfigurationManager.AppSettings["AWSBucketName"],
					Key = awsPath
				};

			if (!bool.Parse(ConfigurationManager.AppSettings["ManagedEncryption"]))
			{
				request.WithServerSideEncryptionMethod(ServerSideEncryptionMethod.AES256);
			}
			else
			{
				request.WithMetaData("customencrypt", "true");
			}

			var responce = S3.PutObject(request);
		}

		public static void EncryptAndUpload(string file, string awsPath, string key)
		{
			if (bool.Parse(ConfigurationManager.AppSettings["ManagedEncryption"]))
			{
				Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(key, appKey);
				using (var aes = new AesCryptoServiceProvider())
				{
					aes.Key = deriveBytes.GetBytes(aes.KeySize / 8);
					aes.IV = deriveBytes.GetBytes(aes.BlockSize / 8);
					using (var temp = new FileStream(file + "_encrypted", FileMode.Create))
					{
						using (var stream = new CryptoStream(new FileStream(file, FileMode.Open), aes.CreateEncryptor(), CryptoStreamMode.Read))
						{
							stream.CopyTo(temp);
						}
					}

					UploadFile(file + "_encrypted", awsPath);

					File.Delete(file + "_encrypted");
				}
			}
			else
				UploadFile(file, awsPath);
		}
	}
}