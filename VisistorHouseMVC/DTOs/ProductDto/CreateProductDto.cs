using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.ProductDto
{
    public class CreateProductDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public long Price { get; set; }

        public IFormFileCollection Files { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public ProductAddress ProductAddress { get; set; }
    }
}