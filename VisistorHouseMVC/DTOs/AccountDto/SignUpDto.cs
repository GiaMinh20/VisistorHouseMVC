using System.ComponentModel.DataAnnotations;

namespace VisistorHouseMVC.DTOs.AccountDto
{
    public class SignUpDto
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Số điện thoạt là bắt buộc")]
        public string Phone { get; set; }

        [Display(Name = "Avatar")]
        public string Avatar { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Không trùng với mật khẩu đã nhập")]
        public string ConfirmPassword { get; set; }
    }
}