using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MongoDB.Driver.Builders;
using ShareGrid.Models.Errors;

namespace ShareGrid.Helpers
{
	public class APIHelp
	{
		public static SortByBuilder GetSortOrder(HttpRequestMessage request, string sort, string[] allowedProperties)
		{
			var properties = sort.Split(',');
			var sortBuilder = new SortByBuilder();

			foreach (string p in properties)
			{
				string[] sortParts = p.Trim().Split('_');
				string sortProperty = sortParts[0];
				string sortOrder = "";
				if (sortParts.Length > 1)
					sortOrder = sortParts[1];

				if (sortProperty == "")
					throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, new APIError("invalidSortProperty", "No sort property specified")));
				else if (!allowedProperties.Contains(sortProperty))
					throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, new APIError("invalidSortProperty", "You are not allowed to sort on '" + sortProperty + "'")));

				if (sortOrder != "" && sortOrder != "asc" && sortOrder != "desc")
					throw new HttpResponseException(request.CreateResponse(HttpStatusCode.BadRequest, new APIError("invalidSortProperty", "Only 'asc' or 'desc' are allowed as a sorting extension")));

				if (sortOrder == "asc")
					sortBuilder.Ascending(sortProperty);
				else
					sortBuilder.Descending(sortProperty);
			}

			return sortBuilder;
		}
	}
}