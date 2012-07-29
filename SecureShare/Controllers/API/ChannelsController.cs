using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Amazon.S3.Model;
using MongoDB.Driver.Builders;
using ShareGrid.Helpers;
using ShareGrid.Models;
using ShareGrid.Models.Errors;

namespace ShareGrid.Controllers.API
{
    public class ChannelsController : ApiController
    {
        [HttpGet]
        public IEnumerable<Channel> GetAllChannels()
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			
			return channels.FindAll();
        }

		[HttpGet]
		[Route(Uri = "check/{channelName}")]
		public object CheckChannelName(string channelName)
		{
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var query = Query.EQ("UniqueName", Channel.GetUniqueName(channelName));
			var channel = channels.FindOne(query);

			if (channel == null)
				return new { available = true };
			else
				return new { available = false };
		}

        [HttpGet]
		[Route(Uri = "{channelName}")]
		public Channel GetChannel(string channelName)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var query = Query.EQ("UniqueName", Channel.GetUniqueName(channelName));
			var channel = channels.FindOne(query);

			if (channel == null)
			{
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "Invalid or non-existant channel")));
			}

			return channels.FindOne(query);
        }

        [HttpPost]
		public SuccessReport RegisterChannel(Channel channel)
		{
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			channel.UpdateUniqueName();

			var query = Query.EQ("UniqueName", channel.UniqueName);
			if (channels.FindOne(query) != null)
			{
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Conflict, new APIError("duplicateName", "There is already a channel with this name")));
			}

			channel.Salt = MongoDBHelper.GetRandomSalt();
			channel.Password = MongoDBHelper.Hash(channel.Password, channel.Salt);
			channel.AdminPassword = MongoDBHelper.Hash(channel.AdminPassword, channel.Salt);

			channel.CreationDate = DateTime.Now;

			channels.Save(channel);

			return new SuccessReport(true);
        }

        [HttpPut]
		[Route(Uri = "{channelName}")]
		public SuccessReport Put(string channelName, AuthenticatedRequest<ChannelUpdate> channelUpdateRequest)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));

			if (channel == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "There's no channel with this name")));

			if (!channelUpdateRequest.Verify(channel, AccessLevel.Admin))
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelKey", "The channel password is invalid")));

			var newData = channelUpdateRequest.Data;
			if (newData.AdminPassword != null)
				channel.AdminPassword = MongoDBHelper.Hash(newData.AdminPassword, channel.Salt);
			if (newData.Password != null)
				channel.Password = MongoDBHelper.Hash(newData.Password, channel.Salt);
			if (newData.Name != null)
				channel.Name = newData.Name;
			if (newData.Description != null)
				channel.Description = newData.Description;

			channels.Save(channel);

			return new SuccessReport(true);
        }

        [HttpDelete]
		[Route(Uri = "{channelName}")]
		public SuccessReport Delete(string channelName, AuthenticatedRequest<object> auth)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));

			if (channel == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "There's no channel with this name")));

			if (!auth.Verify(channel, AccessLevel.Admin))
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelAuth", "The channel authorization data is invalid")));

			channels.Remove(Query.EQ("_id", channel.Id));

			return new SuccessReport(true);
        }

		[HttpPost]
		[Route(Uri = "{channelName}/users")]
		public SuccessReport SubscribeUser(string channelName, AuthenticatedRequest<object> auth)
		{
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");

			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));
			if (channel == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "There's no channel with this name")));

			var accessLevel = auth.VerifyChannelPassword(channel);
			if (accessLevel == AccessLevel.None)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelKey", "The channel password is invalid")));
			
			var user = auth.VerifySessionKey();
			if (user == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidSessionKey", "Your session key is invalid or expired")));

			if (channel.Users == null)
				channel.Users = new List<ChannelUserAccess>();

			var accessInChannel = channel.Users.FirstOrDefault(x => x.Id == user.Id);
			if (accessInChannel != null)
				channel.Users.Remove(accessInChannel);

			channel.Users.Add(new ChannelUserAccess() { Id = user.Id, Access = accessLevel });

			channels.Save(channel);

			return new SuccessReport(true);
		}

		private Channel GetChannelByName(string channelName)
		{
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");

			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));
			if (channel == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "There's no channel with this name")));

			return channel;
		}

		private Tuple<User, Channel, AccessLevel> GetRequestedChannel<T>(string channelName, AuthenticatedRequest<T> entityRequest)
		{
			var channel = GetChannelByName(channelName);

			var access = entityRequest.Verify(channel);
			User user = access.Item1;
			AccessLevel accessLevel = access.Item2;

			if (accessLevel == AccessLevel.None)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelKey", "The channel password is invalid")));
		
			return new Tuple<User, Channel, AccessLevel>(user, channel, accessLevel);
		}

		[HttpGet]
		[Route(Uri = "file/{entityId}/download")]
		public HttpResponseMessage DownloadFile(string entityId, AuthenticatedRequest<object> auth)
		{
			var entities = MongoDBHelper.database.GetCollection<ChannelEntity>("entities");
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");

			var entity = entities.FindOneById(entityId);

			if (entity == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound));

			var channel = channels.FindOneById(entity.ChannelId);

			var access = auth.Verify(channel);
			User user = access.Item1;
			AccessLevel accessLevel = access.Item2;

			if (accessLevel == AccessLevel.None)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden));

			if (channel == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError));

			Stream stream = AWSHelper.GetDecryptedFileStream(entity.FilePathS3, channel.Salt);

			var response = this.Request.CreateResponse();
			response.Content = new StreamContent(stream);

			response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
			if (entity.FileLength != null)
				response.Content.Headers.ContentLength = entity.FileLength;
			response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
			{
				FileName = entity.FileName
			};

			return response;
		}

		[HttpGet]
		[Route(Uri = "file/{entityId}/preview")]
		public HttpResponseMessage PreviewFile(string entityId, AuthenticatedRequest<object> auth)
		{
			var entities = MongoDBHelper.database.GetCollection<ChannelEntity>("entities");
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");

			var entity = entities.FindOneById(entityId);

			if (entity == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound));

			var channel = channels.FindOneById(entity.ChannelId);

			var access = auth.Verify(channel);
			User user = access.Item1;
			AccessLevel accessLevel = access.Item2;

			if (accessLevel == AccessLevel.None)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden));

			if (channel == null)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.InternalServerError));


			var response = this.Request.CreateResponse();
			Stream stream;
			bool isIcon = false;

			if (entity.FilePathPreviewS3 == null || entity.FilePathPreviewS3.Length == 0)
			{
				string extension = entity.FileName.Split('.').Last();
				string iconPath;

				if (!Regex.IsMatch(extension, "[^a-z0-9]", RegexOptions.IgnoreCase))
				{
					iconPath = HttpContext.Current.Server.MapPath("~/Content/icons/" + extension + ".png");

					if (!File.Exists(iconPath))
						iconPath = HttpContext.Current.Server.MapPath("~/Content/icons/unknown.png");
				}
				else
					iconPath = HttpContext.Current.Server.MapPath("~/Content/icons/unknown.png");

				stream = new FileStream(iconPath, FileMode.Open);
				isIcon = true;
			}
			else
			{
				stream = AWSHelper.GetDecryptedFileStream(entity.FilePathPreviewS3, channel.Salt);
				isIcon = false;
			}

			response.Content = new StreamContent(stream);

			response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
			{
				Public = true
			};
			response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
			response.Content.Headers.Expires = new DateTimeOffset(DateTime.Now.AddMonths(11));

			if (!isIcon && entity.FilePreviewLength != null)
				response.Content.Headers.ContentLength = entity.FilePreviewLength;
			else if (isIcon)
				response.Content.Headers.ContentLength = stream.Length;

			return response;
		}

		[HttpPost]
		[Route(Uri = "{channelName}/entities")]
		public ChannelEntity UploadEntity([FromUri] string channelName, AuthenticatedRequest<ChannelEntity> entityRequest)
		{
			try
			{
				Tuple<User, Channel, AccessLevel> reqData = GetRequestedChannel<ChannelEntity>(channelName, entityRequest);
				var user = reqData.Item1;
				var channel = reqData.Item2;
				var accessLevel = reqData.Item3;

				var entity = entityRequest.Data;
				if (user != null)
					entity.UserId = user.Id;

				if (accessLevel != AccessLevel.Admin && entity.Importance == Importance.High)
					entity.Importance = Importance.Normal;

				entity.ResetEmpty();

				if (entity.FileUploads.Count == 0 && entity.Title == null && entity.Message == null)
					throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.BadRequest, new APIError("emptyTitleAndMessage", "At least one of 'Title' and 'Message' must be present")));

				if (entity.FileUploads.Count > 0)
				{
					string awsPath = channel.GetUniqueName() + "_" + channel.Id + "/" +
						MongoDBHelper.Hash(entity.FileUploads.First().Key + "_" + MongoDBHelper.GetRandomSalt());

					AWSHelper.EncryptAndUpload(entity.FileUploads.First().Value, awsPath, channel.Salt);

					entity.FileName = entity.FileUploads.First().Key.Replace("\"", "");
					entity.FileLength = new FileInfo(entity.FileUploads.First().Value).Length;

					if (entity.FileName.EndsWith(".png") || entity.FileName.EndsWith(".jpg") || entity.FileName.EndsWith(".jpeg"))
					{
						try
						{
							using (var fs = new FileStream(entity.FileUploads.First().Value, FileMode.Open))
							{
								// Thumbnail
								ImageResizer.Resize(fs, entity.FileUploads.First().Value + "_thumb", 360, 268);
								try
								{
									AWSHelper.EncryptAndUpload(entity.FileUploads.First().Value + "_thumb", awsPath + "_360x268", channel.Salt);

									entity.FilePathPreviewS3 = awsPath + "_360x268";
									entity.FilePreviewLength = new FileInfo(entity.FileUploads.First().Value + "_thumb").Length;
								}
								finally
								{
									System.IO.File.Delete(entity.FileUploads.First().Value + "_thumb");
								}
							}
						}
						catch { }
					}

					entity.FilePathS3 = awsPath;
				}

				entity.Id = null;
				entity.Date = DateTime.Now;
				entity.ChannelId = channel.Id;

				var entities = MongoDBHelper.database.GetCollection<ChannelEntity>("entities");
				entities.Insert(entity);

				return entity;
			}
			finally
			{
				foreach (var file in entityRequest.Data.FileUploads)
				{
					try
					{
						// Make sure the temp files are removed...
						System.IO.File.Delete(file.Value);
					}
					catch
					{
					}
				}
			}
		}

		[HttpGet]
		[Route(Uri = "{channelName}/entities")]
		public IEnumerable<ChannelEntity> ListEntities(string channelName, AuthenticatedRequest<object> auth,
			[FromUri] int start = 0,
			[FromUri] int limit = 20,
			[FromUri] string sort = "Date_desc"
		)
		{
			var channel = GetChannelByName(channelName);

			if (auth.Verify(channel).Item2 == AccessLevel.None)
				throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelAuth", "The channel authorization data is invalid")));

			var entities = MongoDBHelper.database.GetCollection<ChannelEntity>("entities");
			var query = Query.EQ("ChannelId", channel.Id);

			return entities.Find(query).SetSkip(start).SetLimit(limit).SetSortOrder(
				APIHelp.GetSortOrder(this.Request, sort, new string[] { "UserId", "Title", "Message", "Link", "Date", "Importance" })
			);
		}
    }
}