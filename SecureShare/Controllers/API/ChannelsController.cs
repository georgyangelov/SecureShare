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
		[Route(Uri = "{channelName}")]
		public Channel GetChannel(HttpRequestMessage request, string channelName)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var query = Query.EQ("Name", channelName);
			var channel = channels.FindOne(query);

			if (channel == null)
			{
				throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound, new APIError("invalidChannelName", "Invalid or non-existant channel")));
			}

			return channels.FindOne(query);
        }

        [HttpPost]
		public HttpStatusCode RegisterChannel(HttpRequestMessage request, Channel channel)
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

			channels.Save(channel);

			return HttpStatusCode.Created;
        }

        [HttpPut]
		[Route(Uri = "{channelName}")]
        public HttpStatusCode Put(string channelName, AuthenticatedRequest<ChannelUpdate> channelUpdateRequest)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));

			if (channel == null)
				return HttpStatusCode.NotFound;

			if (!channelUpdateRequest.Verify(channel, UserAccess.Admin))
				return HttpStatusCode.Forbidden;

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

			return HttpStatusCode.OK;
        }

        [HttpDelete]
		[Route(Uri = "{channelName}")]
        public HttpStatusCode Delete(string channelName, AuthenticatedRequest<object> request)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var channel = channels.FindOne(Query.EQ("UniqueName", Channel.GetUniqueName(channelName)));

			if (channel == null)
				return HttpStatusCode.NotFound;

			if (!request.Verify(channel, UserAccess.Admin))
				return HttpStatusCode.Unauthorized;

			channels.Remove(Query.EQ("_id", channel.Id));

			return HttpStatusCode.OK;
        }
    }
}
