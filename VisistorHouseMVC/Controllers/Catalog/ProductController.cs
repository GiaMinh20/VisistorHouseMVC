using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisistorHouseMVC.Data;
using VisistorHouseMVC.Data.Static;
using VisistorHouseMVC.DTOs.ProductDto;
using VisistorHouseMVC.Models;
using VisistorHouseMVC.Services;

namespace VisistorHouseMVC.Controllers.Catalog
{
    public class ProductController : Controller
    {
        private readonly StoreContext _context;
        private readonly ImageService _imageService;
        private readonly IMapper _mapper;
        public ProductController(StoreContext context,
            ImageService imageService,
            IMapper mapper)
        {
            _context = context;
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Catalog()
        {
            var products = await _context.Products.Include(p=>p.ProductAddress).ToListAsync();
            return View(products);
        }


        //Create Product Page
        [Authorize(Roles = "Member")]
        public IActionResult CreateProduct() => View(new CreateProductDto());

        [Authorize(Roles ="Member")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            if (productDto.Files != null)
            {
                product.PictureUrl = new List<string>();
                product.PublicId = new List<string>();

                foreach (var item in productDto.Files)
                {
                    var imageResult = await _imageService.AddImageAsync(item);

                    if (imageResult.Error != null)
                        return BadRequest(new ProblemDetails { Title = imageResult.Error.Message });

                    product.PictureUrl.Add(imageResult.SecureUrl.ToString());
                    product.PublicId.Add(imageResult.PublicId);
                }
            }

            var user = await _context.Users
                    .Include(a => a.UserAddress)
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            if (user != null) product.User = user;

            var productType = await _context.ProductTypes.Where(p=> p.Name == productDto.Type).FirstOrDefaultAsync();
            product.ProductType = productType;
            product.ProductStatus = ProductStatus.Active;
            _context.Products.Add(product);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return RedirectToAction("Profile", "Account");

            return BadRequest(new ProblemDetails { Title = "Xảy ra lỗi khi đăng bài viết" });
        }
        
        //Edit Product Page
        [Authorize]    
        public IActionResult EditProduct(string id)
        {
            var product = _context.Products
                .Include(p => p.ProductAddress)
                .Where(p=>p.Id == id && p.User.UserName == User.Identity.Name)
                .FirstOrDefault();
            if (product == null) return NotFound();
            var editProductDto = _mapper.Map<EditProductDto>(product);
            return View(editProductDto);
        }


    }
}
