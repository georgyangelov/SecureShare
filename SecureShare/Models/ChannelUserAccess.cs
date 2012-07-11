using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShareGrid.Models
{
	public class ChannelUserAccess
	{
		public string Id { get; set; }
		public UserAccess Access { get; set; } 
	}

	public enum UserAccess
	{
		Admin,
		Normal,
		Banned
	}
}