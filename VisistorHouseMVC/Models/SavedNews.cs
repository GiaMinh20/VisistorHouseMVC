using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisistorHouseMVC.Models
{
    public class SavedNews
    {
        [ForeignKey("User")]
        public string Id { get; set; }
        public List<Product> Products { get; set; }
        public virtual User User { get; set; }
    }
}
