using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarTrackAPI.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User ID can not be left blank.")]
        [MaxLength(128)]
        public string UserID { get; set; }
        [Required(ErrorMessage = "User Password can not be left blank.")]
        [MaxLength(16)]
        [MinLength(6)]
        public string UserPassword { get; set; }
        [Required(ErrorMessage = "Email Address can not be left blank.")]
        [MaxLength(100)]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Mobile Number can not be left blank.")]
        [MaxLength(20)]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Full Namecan not be left blank.")]
        [MaxLength(100)]
        public string FullName { get; set; }
    }
}