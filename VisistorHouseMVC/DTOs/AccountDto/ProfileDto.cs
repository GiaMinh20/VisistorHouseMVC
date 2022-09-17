using System.Collections.Generic;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.AccountDto
{
    public class ProfileDto
    {
        public User User { get; set; }
        public List<Product> Products { get; set; }

        public string Address()
        {
            if (User.UserAddress == null)
                return string.Empty;
            return User.UserAddress.Details + " " +
                User.UserAddress.Street + " " +
                User.UserAddress.SubDistrict + " " +
                User.UserAddress.District + " " +
                User.UserAddress.City;
        }
    }
}