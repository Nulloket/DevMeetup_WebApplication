using System.ComponentModel.DataAnnotations;

namespace DevMeeting.Models.Account
{
    public class SignIn : BaseRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool IsPersistant { get; set; }
    }

    public class SignInTwoFactor : BaseRequest
    {
        [Required]
        public string TwoFactorCode { get; set; }
    }
}