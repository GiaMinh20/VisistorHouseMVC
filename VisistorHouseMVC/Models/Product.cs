using System.Collections.Generic;
using VisistorHouseMVC.Data.Static;

namespace VisistorHouseMVC.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Price { get; set; }
        public List<string> PictureUrl { get; set; }
        public List<string> PublicId { get; set; }
        public ProductStatus ProductStatus { get; set; }

        public User User { get; set; }
        public ProductType ProductType { get; set; }
        public virtual List<SavedNews> SavedNews { get; set; }
        public virtual ProductAddress ProductAddress { get; set; }
    }
}