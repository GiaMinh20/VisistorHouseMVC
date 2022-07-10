using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisistorHouseMVC.Data;
using VisistorHouseMVC.Data.Static;
using VisistorHouseMVC.DTOs.RentDto;
using VisistorHouseMVC.Models;
using VisistorHouseMVC.Services;

namespace VisistorHouseMVC.Controllers.Saved
{
    public class SavedNewsController : Controller
    {
        private readonly StoreContext _context;
        private readonly EmailService _emailService;

        public SavedNewsController(StoreContext store,
            EmailService emailService)
        {
            _context = store;
            _emailService = emailService;
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

        [Authorize]
        public async Task<IActionResult> RentNews(string id)
        {
            var product = await _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            var user = await _context.Users
                .Include(u => u.UserAddress)
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var rentInforDto = new RentInforDto
            {
                Product = product,
                User = user
            };
            return View(rentInforDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RentNews(RentInforDto rentInforDto)
        {
            await DeleteItemFromList(rentInforDto.Product.Id);
            var product = await _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Include(p=>p.User)
                .Where(p => p.Id == rentInforDto.Product.Id)
                .FirstOrDefaultAsync();
            rentInforDto.Product.ProductAddress = product.ProductAddress;

            product.ProductStatus = ProductStatus.Đã_thuê;
            _context.Products.Update(product);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                await _emailService.SendEmailAsync(rentInforDto.Product.User.Email,
                "Tin đã được thuê", $"<html><body>" +
                $"<p>Chào {rentInforDto.Product.User.FullName},</p>" +
                $"<p>Tin <strong>{rentInforDto.Product.Name}</strong> của bạn đã được xác nhận thuê bởi <strong>{rentInforDto.User.FullName}</strong></p>" +
                "<p>Thông tin liên lạc với người thuê:</p>" +
                $"<p>Email: {rentInforDto.User.Email} </p>" +
                $"<p>Số điện thoại: {rentInforDto.User.PhoneNumber} </p>" +
                "<p><i>VisistorHouseMVC</i></p>" +
                $"</body></html>");

                await _emailService.SendEmailAsync(rentInforDto.User.Email,
                "Xác nhận tin đã thuê", $"<html><body>" +
                $"<p>Chào {rentInforDto.User.FullName},</p>" +
                "<p>Bạn đã thuê tin tức có thông tin như sau:</p>" +
                $"<p>Tiêu đề: {rentInforDto.Product.Name} </p>" +
                $"<p>Địa chỉ: {rentInforDto.Product.ProductAddress.SumAddress()} </p>" +
                $"<p>Người đăng tin: {rentInforDto.Product.User.FullName} </p>" +
                $"<p>Email: {rentInforDto.Product.User.Email} </p>" +
                $"<p>Số điện thoại:{rentInforDto.Product.User.PhoneNumber} </p>" +
                "Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi," +
                "<p><i>VisistorHouseMVC</i></p>" +
                $"</body></html>");

                return RedirectToAction("Completed", "SavedNews");
            }
            return RedirectToAction("Index", "SavedNews");

        }

        public IActionResult Completed() => View();

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
