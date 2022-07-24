using System;
using VisistorHouseMVC.Data.Static;

namespace VisistorHouseMVC.Models
{
    public class Order
    {
        public int id { get; set; }
        public string RenterId { get; set; }
        public UserAddress ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus OrderStatus { get; set; }
        public Product Product { get; set; }
        public string PaymentIntentId { get; set; }

    }
}
