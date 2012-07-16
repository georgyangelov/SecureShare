using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
		public Channel GetChannel(HttpRequestMessage request, string channelName)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var query = Query.EQ("UniqueName", Channel.GetUniqueName(channelName));
			var channel = channels.FindOne(query);

			if (channel == null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "Invalid or non-existant channel")));
			}

			return channels.FindOne(query);
        }

        [HttpPost]
		public SuccessReport RegisterChannel(HttpRequestMessage request, Channel channel)
		{
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			channel.UpdateUniqueName();

			var query = Query.EQ("UniqueName", channel.UniqueName);
			if (channels.FindOne(query) != null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Conflict, new APIError("duplicateName", "There is already a channel with this name")));
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
		public SuccessReport Put(HttpRequestMessage request, string channelName, AuthenticatedRequest<ChannelUpdate> channelUpdateRequest)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));

			if (channel == null)
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "There's no channel with this name")));

			if (!channelUpdateRequest.Verify(channel, AccessLevel.Admin))
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelKey", "The channel password is invalid")));

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
		public SuccessReport Delete(HttpRequestMessage request, string channelName, AuthenticatedRequest<object> auth)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));

			if (channel == null)
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "There's no channel with this name")));

			if (!auth.Verify(channel, AccessLevel.Admin))
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelKey", "The channel password is invalid")));

			channels.Remove(Query.EQ("_id", channel.Id));

			return new SuccessReport(true);
        }

		[HttpPost]
		[Route(Uri = "{channelName}/users")]
		public SuccessReport SubscribeUser(HttpRequestMessage request, string channelName, AuthenticatedRequest<object> auth)
		{
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");

			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));
			if (channel == null)
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "There's no channel with this name")));

			var accessLevel = auth.VerifyChannelPassword(channel);
			if (accessLevel == AccessLevel.None)
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidChannelKey", "The channel password is invalid")));
			
			var user = auth.VerifySessionKey();
			if (user == null)
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.Forbidden, new APIError("invalidSessionKey", "Your session key is invalid or expired")));

			if (channel.Users == null)
				channel.Users = new List<ChannelUserAccess>();

			var accessInChannel = channel.Users.FirstOrDefault(x => x.Id == user.Id);
			if (accessInChannel != null)
				channel.Users.Remove(accessInChannel);

			channel.Users.Add(new ChannelUserAccess() { Id = user.Id, Access = accessLevel });

			channels.Save(channel);

			return new SuccessReport(true);
		}
    }
}
