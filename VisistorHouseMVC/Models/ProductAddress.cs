using System.ComponentModel.DataAnnotations.Schema;

namespace VisistorHouseMVC.Models
{
    public class ProductAddress : Address
    {
        [ForeignKey("Product")]
        public string Id { get; set; }

        public virtual Product Product { get; set; }
    }
}