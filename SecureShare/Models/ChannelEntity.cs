using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ShareGrid.Helpers;

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

		[JsonIgnore]
		public bool IsEncrypted { get; set; }

		[BsonIgnoreIfNull]
		public string FileName { get; set; }
		[JsonIgnore]
		public string FilePathS3 { get; set; }
		public long? FileLength { get; set; }

		[JsonIgnore]
		public string FilePathPreviewS3 { get; set; }
		public long? FilePreviewLength { get; set; }

		[BsonIgnore]
		public bool IsFile
		{
			get
			{
				return FilePathS3 != null;
			}
		}
		[BsonIgnore]
		public string FileLink
		{
			get
			{
				if (IsFile)
					return ConfigurationManager.AppSettings["ApplicationBaseUrl"] + "/api/channels/file/" + Id + "/download";
				else
					return null;
			}
		}
		[BsonIgnore]
		public string FilePreview
		{
			get
			{
				if (IsFile)
					return ConfigurationManager.AppSettings["ApplicationBaseUrl"] + "/api/channels/file/" + Id + "/preview";
				else
					return null;
			}
		}


		public DateTime Date { get; set; }

		public Importance Importance { get; set; }

		[JsonIgnore]
		[BsonIgnore]
		public IDictionary<string, string> FileUploads { get; set; }

		public void ResetEmpty()
		{
			if (Title == "")
				Title = null;
			if (Message == "")
				Message = null;
			if (Link == "")
				Link = null;
		}

		public void EnsureEncrypted()
		{
			if (bool.Parse(ConfigurationManager.AppSettings["ManagedEncryption"]))
			{
				Title = Crypt.EncryptText(Title, ChannelId);
				Message = Crypt.EncryptText(Message, ChannelId);
				Link = Crypt.EncryptText(Link, ChannelId);

				FileName = Crypt.EncryptText(FileName, ChannelId);
				FilePathS3 = Crypt.EncryptText(FilePathS3, ChannelId);
				FilePathPreviewS3 = Crypt.EncryptText(FilePathPreviewS3, ChannelId);

				IsEncrypted = true;
			}
		}

		public void EnsureDecrypted()
		{
			if (IsEncrypted)
			{
				Title = Crypt.DecryptText(Title, ChannelId);
				Message = Crypt.DecryptText(Message, ChannelId);
				Link = Crypt.DecryptText(Link, ChannelId);

				FileName = Crypt.DecryptText(FileName, ChannelId);
				FilePathS3 = Crypt.DecryptText(FilePathS3, ChannelId);
				FilePathPreviewS3 = Crypt.DecryptText(FilePathPreviewS3, ChannelId);

				IsEncrypted = false;
			}
		}
	}

	public enum Importance
	{
		High = 0,
		Normal = 1,
		Low = 2
	}
}