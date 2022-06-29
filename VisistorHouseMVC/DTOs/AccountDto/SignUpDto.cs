using System.ComponentModel.DataAnnotations;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.AccountDto
{
    public class SignUpDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }

        [Display(Name = "Avatar")]
        public string Avatar { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
