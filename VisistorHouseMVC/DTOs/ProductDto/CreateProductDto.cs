using Microsoft.AspNetCore.Http;
using System;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.ProductDto
{
    public class CreateProductDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public IFormFileCollection Files { get; set; }

        public string Type { get; set; }

        public ProductAddress ProductAddress { get; set; }
    }
}
