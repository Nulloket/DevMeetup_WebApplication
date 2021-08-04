using System;
using System.ComponentModel.DataAnnotations;

namespace DevMeeting.Models.Account
{
    public class SignUp : BaseRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [MaxLength(150)]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }
        [MaxLength(250)] 
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}