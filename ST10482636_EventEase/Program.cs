using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST10482636_EventEase.Data;
using Azure.Storage.Blobs;

namespace ST10482636_EventEase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ST10482636_EventEaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ST10482636_EventEaseContext") ?? throw new InvalidOperationException("Connection string 'ST10482636_EventEaseContext' not found.")));

            // Add Blob Service Client for Azurite Emulation
            builder.Services.AddSingleton(x =>
                new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}