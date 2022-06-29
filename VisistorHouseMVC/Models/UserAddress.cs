using System.ComponentModel.DataAnnotations.Schema;

namespace VisistorHouseMVC.Models
{
    public class UserAddress :Address
    {
        [ForeignKey("User")]
        public string Id { get; set; }
        public virtual User User { get; set; }
    }
}
