using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VisistorHouseMVC.Data;
using VisistorHouseMVC.Data.Static;
using VisistorHouseMVC.DTOs.AccountDto;
using VisistorHouseMVC.Models;
using VisistorHouseMVC.Services;

namespace VisistorHouseMVC.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly StoreContext _context;
        private readonly EmailService _emailService;
        private readonly ImageService _imageService;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            StoreContext context,
            EmailService emailService,
            ImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _imageService = imageService;
        }

        //Sign In Page
        public IActionResult SignIn() => View(new SignInDto());

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDto signInDto)
        {
            if (!ModelState.IsValid) return View(signInDto);

            var user = await _userManager.FindByNameAsync(signInDto.Username);

            if (user != null)
            {
                var userRole = await _userManager.GetRolesAsync(user);

                var passwordCheck = await _userManager.CheckPasswordAsync(user, signInDto.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, signInDto.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (userRole[0] == UserRoles.Admin)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Catalog", "Product");
                        }
                    }
                }
            }
            TempData["Error"] = "Thông tin đăng nhập sai. Vui lòng thử lại!";
            return View(signInDto);
        }


        //Sign Up Page
        public IActionResult SignUp() => View(new SignUpDto());

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
            //if (!ModelState.IsValid) return View(signUpDto);

            var user = await _userManager.FindByNameAsync(signUpDto.Username);
            if (user != null)
            {
                TempData["Error"] = "Tên đăng nhập đã tồn tại";
                return View(signUpDto);
            }
            if (await _context.Users.Where(u => u.Email == signUpDto.EmailAddress).FirstOrDefaultAsync() != null)
            {
                TempData["Error"] = "Email đã được dùng";
                return View(signUpDto);
            }
            if (await _context.Users.Where(u => u.PhoneNumber == signUpDto.Phone).FirstOrDefaultAsync() != null)
            {
                TempData["Error"] = "Số điện thoại đã được dùng";
                return View(signUpDto);
            }
            if (signUpDto.Password != signUpDto.ConfirmPassword)
            {
                TempData["Error"] = "Nhập lại mật khẩu không đúng";
                return View(signUpDto);
            }
            var newUser = new User()
            {
                UserName = signUpDto.Username,
                Email = signUpDto.EmailAddress,
                PhoneNumber = signUpDto.Phone,
                AvatarUrl = "https://res.cloudinary.com/minh20/image/upload/v1656074408/VisitorHouse/default_avatar_m5uoke.png",
                UserAddress = new UserAddress
                {
                    Id = Guid.NewGuid().ToString(),
                    City = "Chưa có thông tin",
                    District = "Chưa có thông tin",
                    SubDistrict = "Chưa có thông tin",
                    Street = "Chưa có thông tin",
                    Details = "Chưa có thông tin"
                }
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, signUpDto.Password);

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.Member);

                //generation of the email token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var link = Url.Action(nameof(VerifyEmail), "Account", new { userId = newUser.Id, token }, Request.Scheme, Request.Host.ToString());

                await _emailService.SendEmailAsync(newUser.Email, "Verify Email", $"<html><body><p>Hi {newUser.UserName},</p><p>Please click here to verify your email:</p><a href=\"{link}\"><strong>Verify Email</strong></a></body></html>");

                TempData["CheckMail"] = "Hãy xác nhận email trước khi đăng nhập";
                return RedirectToAction("SignIn", "Account");

            }
            return View();
        }

        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return RedirectToAction("SignIn", "Account");
            }

            return BadRequest();
        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                TempData["NotFoundEmail"] = "Không tìm thấy tài khoản với Email";
                return RedirectToAction("ForgotPassword", "Account");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action("ResetPassword", "Account", new { email=forgotPasswordDto.Email, token }, Request.Scheme, Request.Host.ToString());
            await _emailService.SendEmailAsync(user.Email, "Reset password", $"<html><body><p>Hi {user.UserName},</p><p>Please click here to redirect to reset password page:</p><a href=\"{link}\"><strong>Click here</strong></a></body></html>");

            TempData["SendEmail"] = "Đã gửi xác nhận đến Email của bạn. Hãy kiểm tra Email";
            return View();
        }

        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }

        //Account Information Page
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile(string name)
        {
            var user = await _context.Users
                    .Include(a => a.UserAddress)
                    .FirstOrDefaultAsync(x => x.UserName == name);

            if (user == null) return NotFound();
            var products = await _context.Products
                .Include(p => p.ProductAddress)
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .Where(p => p.User.Id == user.Id)
                .ToListAsync();
            ProfileDto profile = new ProfileDto
            {
                User = user,
                Products = products
            };

            return View(profile);
        }

        //Edit Profile Page
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditProfile()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                var user = await _context.Users
                    .Include(a => a.UserAddress)
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                var editProfileDto = new EditProfileDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FullName = user.FullName,
                    Dob = user.Dob,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    Address = user.UserAddress
                };
                return View(editProfileDto);
            }
            else { return RedirectToAction("SignIn", "Account"); }
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDto editProfileDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(a => a.UserAddress)
                    .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

                if (editProfileDto.Avatar != null)
                {
                    var imageResult = await _imageService.AddImageAsync(editProfileDto.Avatar);

                    if (imageResult.Error != null)
                        return BadRequest(new ProblemDetails { Title = "Lỗi tải hình ảnh" });

                    if (!string.IsNullOrEmpty(user.PublicId))
                        await _imageService.DeleteImageAsync(user.PublicId);

                    user.AvatarUrl = imageResult.SecureUrl.ToString();
                    user.PublicId = imageResult.PublicId;

                }

                user.FullName = editProfileDto.FullName;
                user.Gender = editProfileDto.Gender;
                user.Dob = editProfileDto.Dob;
                user.UserAddress = new UserAddress
                {
                    Street = editProfileDto.Address.Street,
                    City = editProfileDto.Address.City,
                    Details = editProfileDto.Address.Details,
                    District = editProfileDto.Address.District,
                    SubDistrict = editProfileDto.Address.SubDistrict
                };

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return RedirectToAction("Profile", "Account");

                return BadRequest(new ProblemDetails { Title = "Đã xảy ra lỗi khi chỉnh sửa thông tin" });

            }
            return RedirectToAction("Profile", "Account");
        }

        //Sign Out
        [Authorize]
        public async Task<IActionResult> SignOutUser()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return BadRequest();

            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
