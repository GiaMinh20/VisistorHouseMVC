using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using VisistorHouseMVC.Data;
using VisistorHouseMVC.Helpers;
using VisistorHouseMVC.Models;
using VisistorHouseMVC.Services;

namespace VisistorHouseMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            services.AddDbContext<StoreContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                if (env == "Development")
                {
                    // Use connection string from file.
                    var connUrl = Configuration.GetConnectionString("DefaultConnection");

                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;Trust Server Certificate=true";
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    //var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                    var connUrl = Configuration.GetConnectionString("DefaultConnection");
                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;Trust Server Certificate=true";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);
            });

            services.AddIdentity<User, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<StoreContext>()
                .AddDefaultTokenProviders();

            services.AddMemoryCache();
            services.AddSession();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            services.AddScoped<EmailService>();
            services.AddScoped<ImageService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            //Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                //Admin Controller
                endpoints.MapControllerRoute(
                     name: "admin",
                     defaults: new { controller = "Admin", action = "Index" },
                     pattern: "admin");
                //SavedNews Controller
                endpoints.MapControllerRoute(
                     name: "savednews",
                     defaults: new { controller = "SavedNews", action = "Index" },
                     pattern: "savednews");
                endpoints.MapControllerRoute(
                     name: "rentnews/{id?}",
                     defaults: new { controller = "SavedNews", action = "RentNews" },
                     pattern: "rentnews/{id?}");
                endpoints.MapControllerRoute(
                     name: "rentnews-success",
                     defaults: new { controller = "SavedNews", action = "Completed" },
                     pattern: "rentnews-success");
                //Product Controller
                endpoints.MapControllerRoute(
                     name: "view-product/{id?}",
                     defaults: new { controller = "Product", action = "ViewProduct" },
                     pattern: "view-product/{id?}");
                endpoints.MapControllerRoute(
                     name: "edit-product/{id?}",
                     defaults: new { controller = "Product", action = "EditProduct" },
                     pattern: "edit-product/{id?}");
                endpoints.MapControllerRoute(
                     name: "create-product",
                     defaults: new { controller = "Product", action = "CreateProduct" },
                     pattern: "create-product");
                endpoints.MapControllerRoute(
                     name: "catalog",
                     defaults: new { controller = "Product", action = "Catalog" },
                     pattern: "catalog");
                //Account Controller
                endpoints.MapControllerRoute(
                     name: "edit-profile",
                     defaults: new { controller = "Account", action = "EditProfile" },
                     pattern: "edit-profile");
                endpoints.MapControllerRoute(
                     name: "profile",
                     defaults: new { controller = "Account", action = "Profile" },
                     pattern: "profile");
                endpoints.MapControllerRoute(
                     name: "signin",
                     defaults: new { controller = "Account", action = "SignIn" },
                     pattern: "signin");

                endpoints.MapControllerRoute(
                     name: "signup",
                     defaults: new { controller = "Account", action = "SignUp" },
                     pattern: "signup");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
            InitData.SeedUsersAndRolesAsync(app).Wait();
        }
    }
}
