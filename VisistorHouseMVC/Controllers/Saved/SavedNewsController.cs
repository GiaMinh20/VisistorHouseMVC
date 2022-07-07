using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisistorHouseMVC.Data;
using VisistorHouseMVC.Data.Static;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.Controllers.Saved
{
    public class SavedNewsController : Controller
    {
        private readonly StoreContext _context;
        public SavedNewsController(StoreContext store)
        {
            _context = store;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _context.Users
                .Include(s => s.SavedNews)
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var news = await _context.SavedNews
                .Include(s => s.User)
                .Include(s => s.Products)
                .Where(s => s.Id == user.Id)
                .FirstOrDefaultAsync();
            if (news == null) return NotFound();
            return View(news);
        }

        public async Task<IActionResult> AddItemToList(string id)
        {
            var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            if (user == null) return RedirectToAction("SignIn", "Account");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return BadRequest(new ProblemDetails { Title = "Không tìm thấy tin" });

            
            var savedList = await _context.SavedNews
                .Include(s => s.User)
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.User.Id == user.Id);

            if (savedList == null) savedList = CreateSavedList(product, user);
            else
            {
                savedList.Products.Add(product);
                _context.SavedNews.Update(savedList);
            }
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                TempData["ErrorSave"] = "Đã xảy ra lỗi khi vào lưu danh sách hoặc tin tức đã tồn tại trong danh sách";
                return RedirectToAction("Index", "SavedNews");

            }
            return RedirectToAction("Index", "SavedNews");
        }

        [Authorize]
        public async Task<IActionResult> DeleteItemFromList(string id)
        {
            var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            var product = await _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            var saveNews = await _context.SavedNews.Include(s => s.Products).FirstOrDefaultAsync(s => s.User.Id == user.Id);
            
            saveNews.Products.Remove(product);
            _context.SavedNews.Update(saveNews);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                TempData["ErrorSave"] = "Đã xảy ra lỗi khi xóa tin tức ";
                return RedirectToAction("Index", "SavedNews");

            }
            return RedirectToAction("Index", "SavedNews");
        }

        public async Task<IActionResult> RentNews(string id)
        {
            await DeleteItemFromList(id);
            var product = await _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            product.ProductStatus = ProductStatus.Đã_thuê;
            _context.Products.Update(product);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return RedirectToAction("Index", "SavedNews");

            }
            return RedirectToAction("Index", "SavedNews");
        }
        private SavedNews CreateSavedList(Product product, User user)
        {

            var products = new List<Product>();
            products.Add(product);
            var savedList = new SavedNews
            {
                Id = Guid.NewGuid().ToString(),
                Products = products,
                User = user
            };

            _context.SavedNews.Add(savedList);
            return savedList;
        }
    }
}
