using Microsoft.AspNetCore.Http;

namespace VisistorHouseMVC.DTOs.ProductDto
{
    public class EditProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public long Price { get; set; }

        public IFormFileCollection Files { get; set; }

    }
}
