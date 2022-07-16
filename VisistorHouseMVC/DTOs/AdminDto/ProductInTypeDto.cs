using System.ComponentModel.DataAnnotations;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.DTOs.AdminDto
{
    public class ProductInTypeDto
    {
        [Required(ErrorMessage = "Phải nhập tên danh mục")]
        public string Name { get; set; }
    }
}
