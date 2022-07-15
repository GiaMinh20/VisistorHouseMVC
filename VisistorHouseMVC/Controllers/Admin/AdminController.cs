using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisistorHouseMVC.Data;
using VisistorHouseMVC.Data.Static;
using VisistorHouseMVC.DTOs.AdminDto;
using VisistorHouseMVC.Helpers;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly StoreContext _context;
        private readonly UserManager<User> _userManager;
        public AdminController(StoreContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string searchUser, string searchProductOfUser,
            string sortUser, string sortUserOfProduct, string sortProductCount, string sortProductRented,
            string productType, string productStatus, string sortPrice,
            int pg1 = 1, int pg2 = 1, int pg3 = 1)
        {
            var products = await _context.Products.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var productTypes = await _context.ProductTypes.ToListAsync();
            var productOfUsers = new List<ProductOfUserDto>();

            foreach (var user in users)
            {
                var productOfUser = new ProductOfUserDto();
                productOfUser.UserName = user.UserName;
                var listProductsOfUser = await _context.Products.Where(p => p.User.Id == user.Id).ToListAsync();
                productOfUser.ProductCount = listProductsOfUser.Count();
                foreach (var item in listProductsOfUser)
                {
                    if (item.ProductStatus == ProductStatus.Đã_thuê)
                    {
                        productOfUser.RentedProducts++;
                    }
                }
                productOfUsers.Add(productOfUser);
            }
            //Paging table User
            ViewData["searchUser"] = searchUser;
            if (!String.IsNullOrEmpty(searchUser))
            {
                users = users.Where(u => u.UserName.Contains(searchUser)).ToList();
            }

            ViewData["sortUser"] = sortUser;

            if (!String.IsNullOrEmpty(sortUser))
            {
                switch (sortUser)
                {
                    case "az":
                        users = users.OrderBy(x => x.UserName).ToList();
                        break;
                    case "za":
                        users = users.OrderByDescending(x => x.UserName).ToList();
                        break;
                    default:
                        users = users.ToList();
                        break;
                }
            }

            const int tableUserSize = 5;
            if (pg1 < 1) pg1 = 1;
            int userCount = users.Count();

            var userPager = new Pager(userCount, pg1, tableUserSize);

            int userSkip = (pg1 - 1) * tableUserSize;

            var usersData = users.Skip(userSkip).Take(userPager.Pagesize).ToList();

            this.ViewBag.UserPager = userPager;

            //Paging table Product

            ViewData["productType"] = productType;
            if (!String.IsNullOrEmpty(productType))
            {
                products = products.Where(p => p.ProductType.Name == productType).ToList();
            }

            ViewData["productStatus"] = productStatus;
            if (!String.IsNullOrEmpty(productStatus))
            {
                products = products.Where(p => p.ProductStatus.ToString() == productStatus).ToList();
            }

            ViewData["sortPrice"] = sortPrice;

            if (!String.IsNullOrEmpty(sortPrice))
            {
                switch (sortPrice)
                {
                    case "asc":
                        products = products.OrderBy(x => x.Price).ToList();
                        break;
                    case "desc":
                        products = products.OrderByDescending(x => x.Price).ToList();
                        break;
                    default:
                        products = products.ToList();
                        break;
                }
            }

            const int tableProductSize = 5;
            if (pg2 < 1) pg2 = 1;
            int productCount = products.Count();

            var productPager = new Pager(productCount, pg2, tableProductSize);

            int productSkip = (pg2 - 1) * tableProductSize;

            var productsData = products.Skip(productSkip).Take(productPager.Pagesize).ToList();

            this.ViewBag.ProductPager = productPager;

            //Paging table ProductUser
            ViewData["searchProductOfUser"] = searchProductOfUser;
            if (!String.IsNullOrEmpty(searchProductOfUser))
            {
                productOfUsers = productOfUsers.Where(u => u.UserName.Contains(searchProductOfUser)).ToList();
            }

            ViewData["sortUserOfProduct"] = sortUserOfProduct;

            if (!String.IsNullOrEmpty(sortUserOfProduct))
            {
                switch (sortUserOfProduct)
                {
                    case "az":
                        productOfUsers = productOfUsers.OrderBy(x => x.UserName).ToList();
                        break;
                    case "za":
                        productOfUsers = productOfUsers.OrderByDescending(x => x.UserName).ToList();
                        break;
                    default:
                        productOfUsers = productOfUsers.ToList();
                        break;
                }
            }

            ViewData["sortProductCount"] = sortProductCount;

            if (!String.IsNullOrEmpty(sortProductCount))
            {
                switch (sortProductCount)
                {
                    case "asc":
                        productOfUsers = productOfUsers.OrderBy(x => x.ProductCount).ToList();
                        break;
                    case "desc":
                        productOfUsers = productOfUsers.OrderByDescending(x => x.ProductCount).ToList();
                        break;
                    default:
                        productOfUsers = productOfUsers.ToList();
                        break;
                }
            }

            ViewData["sortProductRented"] = sortProductRented;

            if (!String.IsNullOrEmpty(sortProductRented))
            {
                switch (sortProductRented)
                {
                    case "asc":
                        productOfUsers = productOfUsers.OrderBy(x => x.RentedProducts).ToList();
                        break;
                    case "desc":
                        productOfUsers = productOfUsers.OrderByDescending(x => x.RentedProducts).ToList();
                        break;
                    default:
                        productOfUsers = productOfUsers.ToList();
                        break;
                }
            }
            const int tableProductUserSize = 5;
            if (pg3 < 1) pg3 = 1;
            int productUserCount = productOfUsers.Count();

            var productUserPager = new Pager(productUserCount, pg3, tableProductUserSize);

            int productUserSkip = (pg3 - 1) * tableProductUserSize;

            var productsUserData = productOfUsers.Skip(productUserSkip).Take(productUserPager.Pagesize).ToList();

            this.ViewBag.ProductUserPager = productUserPager;
            var indexDto = new IndexDto
            {
                Users = usersData,
                Products = productsData,
                ProductTypes = productTypes,
                ProductOfUserDtos = productsUserData
            };
            return View(indexDto);
        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            await _userManager.RemoveFromRoleAsync(user, UserRoles.Member.ToUpper());
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> CreateProductType(ProductInTypeDto productInTypeDto)
        {
            if (productInTypeDto == null || String.IsNullOrWhiteSpace(productInTypeDto.Name))
            {
                TempData["FailedCreateProductType"] = "Có lỗi khi thêm danh mục";
                return RedirectToAction("Index", "Admin");
            }

            var productType = new ProductType
            {
                Id = Guid.NewGuid().ToString(),
                Name = productInTypeDto.Name
            };
            foreach (var item in await _context.ProductTypes.ToListAsync())
            {
                if (item.Name == productType.Name)
                {
                    TempData["FailedCreateProductType"] = "Có lỗi khi thêm danh mục";
                    return RedirectToAction("Index", "Admin");
                }
            }
            _context.ProductTypes.Add(productType);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                TempData["FailedCreateProductType"] = "Có lỗi khi thêm danh mục";
                return RedirectToAction("Index", "Admin");
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}
