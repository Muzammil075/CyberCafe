using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CyberCafe.Models
{
    public class loginDetail
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }

    public class RegistrationDetails
    {
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        public string mobile { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        public string cpassword { get; set; }
    }

    public class PaymentDetail
    {
        public string Service { get; set; }
        public string PCNo { get; set; }
        public string Amt { get; set; }
        public string TotalHour { get; set; }       
        public string PayMode { get; set; }
        public string status { get; set; }
    }
}
