using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST10482636_EventEase.Data;
using Azure.Storage.Blobs;
using System;

namespace ST10482636_EventEase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Fetch Database Connection String securely
            builder.Services.AddDbContext<ST10482636_EventEaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ST10482636_EventEaseContext")
                ?? throw new InvalidOperationException("Connection string 'ST10482636_EventEaseContext' not found.")));

            // FLEXIBLE BLOB STORAGE CONFIGURATION: Checks App Settings, Environment Variables, and Connection Strings
            string? storageConnectionString = builder.Configuration["AzureStorage:ConnectionString"]
                                           ?? builder.Configuration["AzureStorage__ConnectionString"]
                                           ?? builder.Configuration.GetConnectionString("AzureStorage")
                                           ?? builder.Configuration["AzureStorage"];

            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new InvalidOperationException("Azure Storage Connection String configuration cannot be located.");
            }

            builder.Services.AddSingleton(x => new BlobServiceClient(storageConnectionString));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // Ensures detailed developer diagnosis pages show up while active in cloud development mode
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}