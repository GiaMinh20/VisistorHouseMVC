using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Authorize]
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
        public async Task<IActionResult> Index(int pg1 = 1, int pg2 = 1, int pg3 = 1)
        {
            var products = await _context.Products.ToListAsync();
            var users = await _context.Users.ToListAsync();
            var productTypes = await _context.ProductTypes.ToListAsync();
            var productOfUsers = new List<ProductOfUserDto>();

            foreach (var user in users)
            {
                var productOfUser = new ProductOfUserDto();
                productOfUser.UserName = user.UserName;
                var lisrProductsOfUser = await _context.Products.Where(p => p.User.Id == user.Id).ToListAsync();
                productOfUser.Products = lisrProductsOfUser;
                productOfUsers.Add(productOfUser);
            }
            //Paging table User
            const int tableUserSize = 5;
            if (pg1 < 1) pg1 = 1;
            int userCount = users.Count();

            var userPager = new Pager(userCount, pg1, tableUserSize);

            int userSkip = (pg1 - 1) * tableUserSize;

            var usersData = users.Skip(userSkip).Take(userPager.Pagesize).ToList();

            this.ViewBag.UserPager = userPager;

            //Paging table Product
            const int tableProductSize = 5;
            if (pg2 < 1) pg2 = 1;
            int productCount = products.Count();

            var productPager = new Pager(productCount, pg2, tableProductSize);

            int productSkip = (pg2 - 1) * tableProductSize;

            var productsData = products.Skip(productSkip).Take(productPager.Pagesize).ToList();

            this.ViewBag.ProductPager = productPager;

            //Paging table ProductUser
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
    }
}
