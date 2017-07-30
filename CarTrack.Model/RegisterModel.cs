using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarTrack.Model
{
    public class AccountInformation : SignUpResponse
    { }
    public class RegisterModel : ServiceRequestBase
    {

        //[Required(ErrorMessage = "Err0001")]//User ID can not be left blank.
        //[MaxLength(128, ErrorMessage = "Err0003")]//User ID length can not be greater than 128 character.
        //[RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Err0007")]//Invalid User ID.
        //public string UserID { get; set; }

        [Required(ErrorMessage = "Err0002")]//User Password can not be left blank.
        [MaxLength(12, ErrorMessage = "Err0004")]//User Password should be between 6 to 10 character.
        [MinLength(6, ErrorMessage = "Err0004")]//User Password should be between 6 to 10 character.
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "Err0012")]//Name can not be left blank.
        [MaxLength(100, ErrorMessage = "Err0013")]//Name can not be greater than 100 character.
        public string FullName { get; set; }

        //[Required(ErrorMessage = "Err0006")]//Mobile Number can not be left blank.
        [MaxLength(20, ErrorMessage = "Err0014")]//Invalid Mobile Numer.
        public string MobileNumber { get; set; }
    }
    public class SignUpResponse : ServiceResponseBase, ISignUpResponse
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

    public interface ISignUpResponse
    {
        string UserID { get; set; }

        string FullName { get; set; }

        string MobileNumber { get; set; }

        string EmailAddress { get; set; }

        string AuthToken { get; set; }

        string OTP { get; set; }

        bool? IsVerified { get; set; }

        bool? IsLocked { get; set; }

        DateTime? LastLoginDate { get; set; }

        List<CarModel> MyCars { get; set; }

        UserSetting UserSettings { get; set; }

    }
    public class UserSetting : ServiceRequestBase
    {

        public string PurchaseUrl { get; set; }

        public int NotificationInterval { get; set; }

        public string NotificationLanguage { get; set; }

        public decimal NotificationDistance { get; set; }

        //public string UserID { get; set; }

        public string AboutUsUrl { get; set; }

        public decimal? MinDistance { get; set; }

        public decimal? MaxDistance { get; set; }

        public decimal? MinRefreshInterval { get; set; }

        public decimal? MaxRefreshInterval { get; set; }
    }
    public class UserOTP : ServiceResponseBase
    {
        public string UserID { get; set; }
        public string OTP { get; set; }
        public bool? IsExpired { get; set; }
        public DateTime? OTPDate { get; set; }

    }
    public class UserFeedback : ServiceRequestBase
    {
        //[Required(ErrorMessage = "Err0001")]//User ID can not be left blank.
        //[MaxLength(128, ErrorMessage = "Err0003")]//User ID length can not be greater than 128 character.
        //public string UserID { get; set; }

        [Required(ErrorMessage = "Err0021")]//Feedback can not be left blank.
        [MaxLength(200, ErrorMessage = "Err0022")]//Feedback length can not be greater than 200 character.
        public string FeedbackText { get; set; }

        public DateTime? FeedbackDate { get; set; }

        //[Required(ErrorMessage = "Err0023")]//Rating can not be left blank.
        [Range(1, 5, ErrorMessage = "Err0024")]//Rating should be between 1 to 5.
        public int? Rating { get; set; }

        //[Required(ErrorMessage = "Err0025")]//User ID can not be left blank.
        public string RatingText { get; set; }

    }
    public class VerifyUser : ServiceRequestBase
    {
        [Required(ErrorMessage = "Err0001")]
        public string OTP { get; set; }
    }
    public class ChangePassword : ServiceRequestBase
    {
        public string NewPassword { get; set; }

        public string OTP { get; set; }
    }
}