using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShareGrid.Models
{
    public class SuccessReport
    {
		public bool Success { get; set; }

		public SuccessReport(bool success)
		{
			Success = success;
		}
    }
}
