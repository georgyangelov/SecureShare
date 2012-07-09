using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace ShareGrid.Models
{
	[ModelBinder(typeof(ShareGrid.Helpers.ModelBinders.AuthenticatedRequestProvider))]
	public class AuthenticatedRequest<TData>
	{
		public TData Data { get; set; }

		public AuthType AuthType { get; set; }
		public string Key { get; set; }
	}

	public enum AuthType
	{
		SessionKey,
		ChannelPassword,
		NotAuthenticated
	}
}