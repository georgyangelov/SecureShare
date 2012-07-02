using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShareGrid.Models
{
	public class SessionKey
	{
		public string Key { get; set; }
		public DateTime Expires { get; set; }

		// Generate a new unique session key
		public SessionKey(User user, DateTime expires)
		{
			Key = MongoDBHelper.Hash(expires.ToString() + Guid.NewGuid().ToString() + user.Password + user.Salt);
			Expires = expires;
		}
	}
}