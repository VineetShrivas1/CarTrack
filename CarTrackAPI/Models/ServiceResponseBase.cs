using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarTrackAPI.Models
{
    public class ServiceResponseBase
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorFound { get; set; }
    }
}