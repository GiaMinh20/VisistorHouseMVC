using System.ComponentModel.DataAnnotations;

namespace VisistorHouseMVC.DTOs.AccountDto
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password ")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không trùng với mật khẩu đã nhập")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}