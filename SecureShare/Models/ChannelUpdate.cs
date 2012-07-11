using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShareGrid.Models
{
	public class ChannelUpdate
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Password { get; set; }
		public string AdminPassword { get; set; }
	}
}