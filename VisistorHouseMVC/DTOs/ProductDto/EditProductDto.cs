using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using VisistorHouseMVC.Data.Static;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.ProductDto
{
    public class EditProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }
        public ProductStatus ProductStatus { get; set; }
        public IFormFileCollection Files { get; set; }
        public Address ProductAddress { get; set; }
        public List<string> ProductImages { get; set; }
    }
}