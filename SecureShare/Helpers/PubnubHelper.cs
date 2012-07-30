using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ShareGrid.Helpers.PubnubAPI;

namespace ShareGrid.Helpers
{
	public class PubnubHelper
	{
		public static pubnub API;

		static PubnubHelper()
		{
			API = new pubnub(ConfigurationManager.AppSettings["PubnubPublishKey"],
					ConfigurationManager.AppSettings["PubnubSubscribeKey"],
					ConfigurationManager.AppSettings["PubnubSecretKey"],
					true);
		}

		public static void Publish(string channelUniqueName, object message)
		{
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("channel", "channel_" + channelUniqueName);
			args.Add("message", message);
			API.Publish(args);
		}
	}
}