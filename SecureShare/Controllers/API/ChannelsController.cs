using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MongoDB.Driver.Builders;
using ShareGrid.Helpers;
using ShareGrid.Models;

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
        public Channel GetChannel(string channelName)
        {
			var channels = MongoDBHelper.database.GetCollection<Channel>("channels");
			var query = Query.EQ("Name", channelName);

			return channels.FindOne(query);
        }

        // POST api/channels
		[HttpPost]
        public Channel RegisterChannel(HttpRequestMessage request, Channel channel)
        {
			throw new NotImplementedException();
        }

        // PUT api/channels/5
		[HttpPut]
        public SuccessReport Put(int id, string value)
        {
			throw new NotImplementedException();
        }

        // DELETE api/channels/5
		[HttpDelete]
        public SuccessReport Delete(int id)
        {
			throw new NotImplementedException();
        }
    }
}
