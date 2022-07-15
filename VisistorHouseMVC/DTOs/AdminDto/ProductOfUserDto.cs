using System.Collections.Generic;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.AdminDto
{
    public class ProductOfUserDto
    {
        public string UserName { get; set; }
        public int ProductCount { get; set; }
        public int RentedProducts { get; set; }=0;
    }
}
