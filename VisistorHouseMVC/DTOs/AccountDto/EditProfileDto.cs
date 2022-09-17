using Microsoft.AspNetCore.Http;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.AccountDto
{
    public class EditProfileDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        public Address Address { get; set; }
        public IFormFile Avatar { get; set; }
        public string AvatarUrl { get; set; }
        public string PhoneNumber { get; set; }
    }
}