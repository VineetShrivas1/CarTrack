using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarTrack.Model
{
    public class ServiceResponseBase
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorFound { get; set; }
    }
    public class ServiceRequestBase
    {
        [Required(ErrorMessage = "Err0001")]//User ID can not be left blank.
        [MaxLength(128, ErrorMessage = "Err0003")]//User ID length can not be greater than 128 character.
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Err0007")]//Invalid User ID.
        public string UserID { get; set; }
        public string IPAddress { get; set; }
        public string Source { get; set; }
        public string ApiVersion { get; set; }
        public string UserDeviceID { get; set; }
        //public DateTime? RequestDate { get; set; }
        //public DateTime? ResponseDate { get; set; }
    }
}