using System;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookStore.Service;
using BookStore.Service.Book;
using BookStore.Service.BookService;
using BookStore.Service.Account;
using BookStore.Service.Wishlist;
using BookStore.Service.Category;
using BookStore.Service.Review;
using BookStore.Service.Cart;
using BookStore.Service.Email;
using BookStore.Service.Order;
using BookStore.Service.User;

namespace BookStore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure Entity Framework and SQL Server
            //string connectionString = @"Server=.; Database=BookStore; Integrated Security=True; TrustServerCertificate = True;";
            builder.Services.AddDbContext<BookStoreDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<BookStoreDBContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));


            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using(var scope = app.Services.CreateScope())
            {
                DataSeeder.SeedRolesAndAdmin(scope.ServiceProvider);
            }

            app.Run();
        }
    }
}
