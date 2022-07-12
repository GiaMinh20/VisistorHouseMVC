using System.Collections.Generic;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.AdminDto
{
    public class ProductOfUserDto
    {
        public string UserName { get; set; }
        public List<Product> Products { get; set; }
    }
}
