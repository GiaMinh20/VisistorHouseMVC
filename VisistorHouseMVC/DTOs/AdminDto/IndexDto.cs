using System.Collections.Generic;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.AdminDto
{
    public class IndexDto
    {
        public List<User> Users { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductType> ProductTypes { get; set; }
        public List<ProductOfUserDto> ProductOfUserDtos { get; set; }
    }
}
