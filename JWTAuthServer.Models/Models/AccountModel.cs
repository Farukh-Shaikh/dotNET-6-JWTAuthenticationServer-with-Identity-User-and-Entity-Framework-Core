using System.ComponentModel.DataAnnotations;

namespace JWTAuthServer.Common.Models
{
    public class AccountModel
    {
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        //[Display(Name = "Email")]
        public string Email { get; set; }
        public string clientId { get; set; }

        public string Source { get; set; }
        public string ResetPasswordUrl { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        //[Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Code { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        //[Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        //[Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeUserEmailModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }

    }
    public class ChangeUserPhoneNumberModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

    }

    public class ChangeUserPhoneOrEmailModel
    {
        [Required]
        public string Id { get; set; }

        //[Required]
        public string PhoneNumber { get; set; }

        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        //[Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        //[Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class TokenRequestModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string access_token { get; set; }
        public DateTime? expires { get; set; }
        [Required]
        public string refresh_token { get; set; }

    }

}
