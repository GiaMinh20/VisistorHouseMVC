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
using VisistorHouseMVC.Helpers;
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

        public async Task<IActionResult> Catalog(string sort, string searchString, string type, int pg = 1)
        {
            ViewData["searchString"] = searchString;
            ViewData["sort"] = sort;
            ViewData["type"] = type;

            var products = await _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .Where(p => p.ProductStatus == ProductStatus.Chưa_thuê)
                .ToListAsync();
            if (!String.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "az":
                        products = products.OrderBy(x => x.Name).ToList();
                        break;
                    case "za":
                        products = products.OrderByDescending(x => x.Name).ToList();
                        break;
                    case "highest":
                        products = products.OrderByDescending(x => x.Price).ToList();
                        break;
                    case "lowest":
                        products = products.OrderBy(x => x.Price).ToList();
                        break;
                    default:
                        products = products.ToList();
                        break;
                }
            }

            if (!String.IsNullOrEmpty(type))
            {
                products = products.Where(p => p.ProductType.Name == type).ToList();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) ||
                    p.ProductType.Name.Contains(searchString) ||
                    p.ProductAddress.City.Contains(searchString) ||
                    p.ProductAddress.District.Contains(searchString) ||
                    p.ProductAddress.SubDistrict.Contains(searchString) ||
                    p.ProductAddress.Street.Contains(searchString) ||
                    p.ProductAddress.Details.Contains(searchString))
                    .ToList();
            }
            const int pageSize = 6;
            if (pg < 1) pg = 1;

            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);

            int recsSkip = (pg - 1) * pageSize;
            var data = products.Skip(recsSkip).Take(pager.Pagesize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }


        //Create Product Page
        [Authorize(Roles = "Member")]
        public IActionResult CreateProduct() => View(new CreateProductDto());

        [Authorize(Roles = "Member")]
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

            var productType = await _context.ProductTypes.Where(p => p.Name == productDto.Type).FirstOrDefaultAsync();
            product.ProductType = productType;
            product.ProductStatus = ProductStatus.Chưa_thuê;
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
                .Where(p => p.Id == id && p.User.UserName == User.Identity.Name)
                .FirstOrDefault();
            if (product == null) return NotFound();
            var editProductDto = _mapper.Map<EditProductDto>(product);
            editProductDto.ProductImages = product.PictureUrl;
            return View(editProductDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProduct(EditProductDto editProductDto)
        {
            var product = _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .Where(p => p.Id == editProductDto.Id && p.User.UserName == User.Identity.Name)
                .FirstOrDefault();

            product.Name = editProductDto.Name;
            product.Description = editProductDto.Description;
            product.Price = editProductDto.Price;
            product.ProductAddress.City = editProductDto.ProductAddress.City;
            product.ProductAddress.District = editProductDto.ProductAddress.District;
            product.ProductAddress.SubDistrict = editProductDto.ProductAddress.SubDistrict;
            product.ProductAddress.Street = editProductDto.ProductAddress.Street;
            product.ProductAddress.Details = editProductDto.ProductAddress.Details;
            product.ProductStatus = editProductDto.ProductStatus;

            if (editProductDto.Files != null)
            {
                product.PictureUrl = new List<string>();
                product.PublicId = new List<string>();

                foreach (var item in editProductDto.Files)
                {
                    var imageResult = await _imageService.AddImageAsync(item);

                    if (imageResult.Error != null)
                        return BadRequest(new ProblemDetails { Title = imageResult.Error.Message });

                    product.PictureUrl.Add(imageResult.SecureUrl.ToString());
                    product.PublicId.Add(imageResult.PublicId);
                }
            }

            _context.Products.Update(product);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                TempData["ErrorEdit"] = "Đã xảy ra lỗi khi chỉnh sửa sản phẩm";
                return RedirectToAction("EditProduct", "Product");

            }
            return RedirectToAction("Profile", "Account");

        }

        public async Task<ActionResult> DeleteProduct(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                TempData["ErrorDeleteProduct"] = "Đã xảy ra lỗi xóa tin";
                return RedirectToAction("EditProduct", "Product");

            }
            return RedirectToAction("Profile", "Account", new {name=User.Identity.Name});
        }
        public IActionResult ViewProduct(string id)
        {
            var product = _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .Where(p => p.Id == id)
                .FirstOrDefault();
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
