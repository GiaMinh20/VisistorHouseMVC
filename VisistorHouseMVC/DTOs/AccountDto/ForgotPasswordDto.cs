using System.ComponentModel.DataAnnotations;

namespace VisistorHouseMVC.DTOs.AccountDto
{
    public class ForgotPasswordDto
    {
        [Required]
        public string Email { get; set; }
    }
}