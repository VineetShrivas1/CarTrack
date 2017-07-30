using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarTrack.Model
{
    public class LoginModel:ServiceRequestBase
    {
        //[Required(ErrorMessage = "Err0001")]//User ID can not be left blank.
        //[MaxLength(128, ErrorMessage = "Err0003")]//User ID length can not be greater than 128 character.
        //[RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Err0007")]//Invalid User ID.
        //public string UserID { get; set; }

        [Required(ErrorMessage = "Err0002")]//User Password can not be left blank.
        //[MaxLength(12, ErrorMessage = "Err0004")]//User Password should be between 6 to 10 character.
        //[MinLength(6, ErrorMessage = "Err0004")]//User Password should be between 6 to 10 character.
        public string UserPassword { get; set; }
    }
    public class LoginResponse : ServiceResponseBase, ISignUpResponse
    {
        public string UserID { get; set; }

        public string FullName { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string AuthToken { get; set; }

        public string OTP { get; set; }

        public bool? IsVerified { get; set; }

        public bool? IsLocked { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public List<CarModel> MyCars { get; set; }

        public UserSetting UserSettings { get; set; }
    }
}