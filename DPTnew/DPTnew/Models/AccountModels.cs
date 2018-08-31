using DPTnew.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace DPTnew.Models
{


    [Table("DPT_People")]
    public class Contact
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string OriginalPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Language { get; set; }
        public int Status { get; set; }
        public string EmailStatus { get; set; }
        public string AccountNumber { get; set; }
        public string PrimaryContact { get; set; }

        public virtual Company Company { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource),
        ErrorMessageResourceName = "RequiredFieldPassword")]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword", ResourceType = typeof(Resource))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource),
        ErrorMessageResourceName = "RequiredFieldPassword")]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Resource))]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [StringLength(100, ErrorMessageResourceType = typeof(Resource),
        ErrorMessageResourceName = "LengthPassword", MinimumLength = 8)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof(Resource))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resource),
            ErrorMessageResourceName = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Please Enter Email Address")]
        [Display(Name = "Username (Email Address)")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class LostPasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ResetPasswordMsg")]
        [Display(Name = "AccountMail", ResourceType = typeof(Resource))]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MailErrMsg", ErrorMessage = null)]
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "New password and confirmation does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ReturnToken { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Please Enter Email Address")]
        [Display(Name = "User name (Email Address)")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "Language")]
        public string Language { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Email Status")]
        public int EmailStatus { get; set; }

        [Required]
        [Display(Name = "AccountNumber")]
        public string AccountNumber { get; set; }

    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
