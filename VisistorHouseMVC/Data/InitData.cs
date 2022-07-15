using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisistorHouseMVC.Data.Static;
using VisistorHouseMVC.Models;

namespace VisistorHouseMVC.Data
{
    public class InitData
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                }
                if (!await roleManager.RoleExistsAsync(UserRoles.Member))
                {
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Member));
                }

                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string adminEmail = "admin@visitorhouse.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        UserName = "Admin",
                        Email = adminEmail,
                        AvatarUrl= "https://res.cloudinary.com/minh20/image/upload/v1656074408/VisitorHouse/default_avatar_m5uoke.png",
                        EmailConfirmed = true

                    };
                    await userManager.CreateAsync(newAdminUser, "Admin@123");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                var context = serviceScope.ServiceProvider.GetService<StoreContext>();

                context.Database.EnsureCreated();

                if (context.ProductTypes.Any()) return;
                var productTypes = new List<ProductType>
                {
                    new ProductType
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name ="Nhà trọ"
                    },
                    new ProductType
                    {Id = Guid.NewGuid().ToString(),
                        Name="Chung cư"
                    },
                    new ProductType
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Nhà ở"
                    }
                };
                foreach (var item in productTypes)
                {
                    context.ProductTypes.Add(item);
                }
                context.SaveChanges();
            }
        }
    }
}
