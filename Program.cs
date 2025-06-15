using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using MiniPhotoshop.Backend.Services;
using MiniPhotoshop.Backend.Filters;
using MiniPhotoshop.Backend.Filters.BasicFilters;
using MiniPhotoshop.Backend.Filters.TransformFilters;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;

namespace MiniPhotoshop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Thêm services vào container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.EnableEndpointRouting = false;
            })
            .AddRazorOptions(options =>
            {
                // Cấu hình lại đường dẫn tìm view trong thư mục Frontend/views
                options.ViewLocationFormats.Clear();
                options.ViewLocationFormats.Add("/Frontend/views/{1}/{0}" + RazorViewEngine.ViewExtension);
                options.ViewLocationFormats.Add("/Frontend/views/Shared/{0}" + RazorViewEngine.ViewExtension);
            });
            builder.Services.AddRazorPages();

            // Add session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add SixLabors.ImageSharp.Web services
            builder.Services.AddImageSharp();

            // Đăng ký các dịch vụ
            builder.Services.AddSingleton<FileStorageService>();
            builder.Services.AddSingleton<ImageProcessingService>();

            // Đăng ký các bộ lọc cơ bản
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.GrayscaleFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.SepiaFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.InvertFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.BrightnessFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.ContrastFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.HueFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.SaturationFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.GammaFilter>();

            // Đăng ký các bộ lọc biến đổi
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.FlipHorizontalFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.FlipVerticalFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.RotateFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.ResizeFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.CropFilter>();

            // Đăng ký các bộ lọc làm mờ và sắc nét
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.BlurFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.GaussianBlurFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.BoxBlurFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.SharpenFilter>();

            // Đăng ký các bộ lọc hiệu ứng đặc biệt
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.PixelateFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.OilPaintFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.LomographFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.PolaroidFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.KodachromeFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.GlowFilter>();
            builder.Services.AddSingleton<IImageFilter, MiniPhotoshop.Backend.Filters.VignetteFilter>();

            // Thêm CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseImageSharp();
            app.UseRouting();

            // Add session middleware
            app.UseSession();

            app.UseCors("AllowAll");
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "", controller = "Home", action = "Index" },
                constraints: null,
                dataTokens: new { Namespace = "MiniPhotoshop.Backend.Controllers" });

            // Đảm bảo thư mục uploads tồn tại
            var uploadsDir = Path.Combine(app.Environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            app.Run();
        }
    }
} 