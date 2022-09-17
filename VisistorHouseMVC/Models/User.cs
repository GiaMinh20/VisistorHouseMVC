using Microsoft.AspNetCore.Identity;

namespace VisistorHouseMVC.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string AvatarUrl { get; set; }
        public string Gender { get; set; }
        public string Dob { get; set; }
        public string PublicId { get; set; }
        public virtual SavedNews SavedNews { get; set; }
        public virtual UserAddress UserAddress { get; set; }
    }
}