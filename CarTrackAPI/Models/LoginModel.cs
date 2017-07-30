using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CarTrackAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User ID can not be left blank.")]
        [MaxLength(128)]
        public string UserID { get; set; }
        [Required(ErrorMessage = "User Password can not be left blank.")]
        [MaxLength(16)]
        [MinLength(6)]
        public string UserPassword { get; set; }
    }
}