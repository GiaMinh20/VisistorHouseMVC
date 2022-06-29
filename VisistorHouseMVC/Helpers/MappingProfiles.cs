using AutoMapper;
using VisistorHouseMVC.DTOs.ProductDto;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //AccountController
            //CreateMap<EditUserDto, User>();

            //ProductController
            CreateMap<CreateProductDto, Product>();
            CreateMap<EditProductDto, Product>();
            CreateMap<Product, EditProductDto>();
        }
    }
}
