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
        // GET api/channels
		[HttpGet]
        public IEnumerable<Channel> GetAllChannels()
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			
			return channels.FindAll();
        }

        // GET api/channels/{id}
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

        // POST api/channels
		[HttpPost]
        public HttpResponseMessage RegisterChannel(HttpRequestMessage request, Channel channel)
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

			return new HttpResponseMessage(HttpStatusCode.Created);
        }

        // PUT api/channels/5
		[HttpPut]
		[Route(Uri = "{channelName}")]
        public void Put(string channelName, AuthenticatedRequest<ChannelUpdate> channelUpdateRequest)
        {
			
        }

        // DELETE api/channels/5
		[HttpDelete]
        public SuccessReport Delete(int id)
        {
			throw new NotImplementedException();
        }
    }
}
